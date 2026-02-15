using System.Data;
using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Caching;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Persistence;

namespace Gamma.Kernel.Data;

public sealed class GenericRepository<TEntity>(
    ICurrentUser currentUser,
    IUidGenerator uidGenerator,
    ISystemClock clock
) : IRepository<TEntity>
    where TEntity : BaseEntity, new()
{
    private const string ID_FIELD = "Id";
    private static IUnitOfWork Uow => UnitOfWorkScope.Current;

    #region CUD Operations

    public async ValueTask<TEntity> InsertAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.NormalizeStringProperties();
        if (entity.Id == Guid.Empty) entity.Id = uidGenerator.New();
        SetAuditFields(entity, ChangeAction.Insert);

        var dialect = SqlDialectResolver.Resolve(Uow.Connection);
        var cache = EntityPropertyCache<TEntity>.Instance;
        var props = cache.InsertableProperties;

        var columns = string.Join(", ", props.Select(p => dialect.EscapeIdentifier(p.Name)));
        var parameters = string.Join(", ", props.Select(p => "@" + p.Name));

        var tableName = GetTableName(dialect);
        var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

        if (cache.IdentityProperty is not null)
        {
            sql += $" {dialect.GetLastInsertIdSql()}";
            var id = await Uow.Connection.ExecuteScalarAsync(sql, entity, Uow.Transaction, commandTimeout: dialect.DefaultCommandTimeout);
            entity.SetPropertyValue(cache.IdentityProperty.Name, id);
        }
        else
        {
            await Uow.Connection.ExecuteAsync(sql, entity, Uow.Transaction, commandTimeout: dialect.DefaultCommandTimeout);
        }

        return entity;
    }

    public async ValueTask<int> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.NormalizeStringProperties();
        SetAuditFields(entity, ChangeAction.Update);

        var dialect = SqlDialectResolver.Resolve(Uow.Connection);
        var cache = EntityPropertyCache<TEntity>.Instance;
        var rowVersionProp = cache.RowVersionProperty;

        var setParts = cache.UpdatableProperties
                            .Where(p => p != rowVersionProp)
                            .Select(p => $"{dialect.EscapeIdentifier(p.Name)} = @{p.Name}")
                            .ToList();

        if (rowVersionProp != null)
        {
            var col = dialect.EscapeIdentifier(rowVersionProp.Name);
            setParts.Add($"{col} = {col} + 1");
        }

        var setClause = string.Join(", ", setParts);
        var tableName = GetTableName(dialect);
        var idColumn = dialect.EscapeIdentifier(ID_FIELD);

        var sql = $"UPDATE {tableName} SET {setClause} WHERE {idColumn} = @Id";
        if (rowVersionProp != null)
        {
            var rowVersionColumn = dialect.EscapeIdentifier(rowVersionProp.Name);
            sql += $" AND {rowVersionColumn} = @{rowVersionProp.Name}";
        }

        var affected = await Uow.Connection.ExecuteAsync(sql, entity, Uow.Transaction, commandTimeout: dialect.DefaultCommandTimeout);

        if (affected == 0 && rowVersionProp != null)
            throw new DBConcurrencyException($"Entity update failed due to concurrency violation. Type: {typeof(TEntity).Name}, Id: {entity.Id}");

        if (rowVersionProp != null)
            IncrementRowVersion(entity);

        return affected;
    }

    public async ValueTask<int> DeleteByIdAsync<TKey>(TKey id, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        var dialect = SqlDialectResolver.Resolve(Uow.Connection);
        var tableName = GetTableName(dialect);
        var idColumn = dialect.EscapeIdentifier(ID_FIELD);

        var sql = $"DELETE FROM {tableName} WHERE {idColumn} = @Id";
        var affected = await Uow.Connection.ExecuteAsync(sql, new { Id = id }, Uow.Transaction, commandTimeout: dialect.DefaultCommandTimeout);

        return affected;
    }
    #endregion

    #region Helpers

    private static void IncrementRowVersion(TEntity entity)
    {
        var prop = EntityPropertyCache<TEntity>.Instance.RowVersionProperty;
        if (prop == null) return;

        var value = prop.GetValue(entity);
        if (value == null) return;

        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        if (!typeof(IConvertible).IsAssignableFrom(type))
            throw new InvalidOperationException($"RowVersion property '{prop.Name}' must be numeric but found '{type.Name}'");

        var nextValue = Convert.ChangeType(Convert.ToInt64(value) + 1, type);
        prop.SetValue(entity, nextValue);
    }

    private void SetAuditFields(TEntity entity, ChangeAction action)
    {
        var actor = currentUser.GetActor();
        var now = clock.Now;

        if (action == ChangeAction.Insert && entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        if (action == ChangeAction.Insert)
            entity.LogCreatedBy(actor, now);

        entity.LogModifiedBy(actor, now);
    }

    private static string GetTableName(ISqlDialect dialect)
    {
        var tableName = EntityPropertyCache<TEntity>.Instance.TableName;
        return tableName.Contains('.')
            ? string.Join(".", tableName.Split('.').Select(p => dialect.EscapeIdentifier(p)))
            : dialect.EscapeIdentifier(tableName);
    }
    #endregion
}
