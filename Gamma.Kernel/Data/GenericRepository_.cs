using System.Data;
using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Caching;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Extensions;

namespace Gamma.Kernel.Data;

public sealed class GenericRepository_<TEntity>(
    ICurrentUser currentUser,
    ISystemClock clock) : IRepository_<TEntity>
    where TEntity : BaseEntity, new()
{
    private const string ID_FIELD = "Id";

    #region CUD Operations

    public async Task<TEntity> InsertAsync(
        IUnitOfWork uow,
        TEntity entity,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(uow);

        entity.NormalizeStringProperties();
        SetAuditFields(entity, ChangeAction.Insert);

        var dialect = SqlDialectResolver.Resolve(uow.Connection);
        var cache = EntityPropertyCache<TEntity>.Instance;
        var props = cache.InsertableProperties;

        var columns = string.Join(", ", props.Select(p => dialect.EscapeIdentifier(p.Name)));
        var parameters = string.Join(", ", props.Select(p => "@" + p.Name));

        var tableName = GetTableName(dialect);
        var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

        var cmdDef = new CommandDefinition(
            sql,
            entity,
            uow.Transaction,
            commandTimeout: dialect.DefaultCommandTimeout,
            cancellationToken: ct);

        if (cache.IdentityProperty is not null)
        {
            sql = $"{sql} {dialect.GetLastInsertIdSql()}";
            var cmdDefWithId = new CommandDefinition(
                sql,
                entity,
                uow.Transaction,
                commandTimeout: dialect.DefaultCommandTimeout,
                cancellationToken: ct);

            var id = await uow.Connection.ExecuteScalarAsync(cmdDefWithId);
            entity.SetPropertyValue(cache.IdentityProperty.Name, id);
        }
        else
        {
            await uow.Connection.ExecuteAsync(cmdDef);
        }

        return entity;
    }

    public async Task<int> UpdateAsync(
    IUnitOfWork uow,
    TEntity entity,
    CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(uow);

        entity.NormalizeStringProperties();
        SetAuditFields(entity, ChangeAction.Update);

        var dialect = SqlDialectResolver.Resolve(uow.Connection);
        var cache = EntityPropertyCache<TEntity>.Instance;
        var rowVersionProp = cache.RowVersionProperty;

        // Build SET clause (exclude RowVersion)
        var setParts = cache.UpdatableProperties
                            .Where(p => p != rowVersionProp)
                            .Select(p => $"{dialect.EscapeIdentifier(p.Name)} = @{p.Name}")
                            .ToList();

        // Increment RowVersion in SQL
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

        var cmdDef = new CommandDefinition(
            sql,
            entity,
            uow.Transaction,
            commandTimeout: dialect.DefaultCommandTimeout,
            cancellationToken: ct);

        var affected = await uow.Connection.ExecuteAsync(cmdDef);

        if (affected == 0 && rowVersionProp != null)
        {
            throw new DBConcurrencyException(
                $"Entity update failed due to concurrency violation. " +
                $"Type: {typeof(TEntity).Name}, Id: {entity.Id}");
        }

        if (rowVersionProp != null)
        {
            uow.OnCommitted(ct =>
            {
                IncrementRowVersion(entity);
                return Task.CompletedTask;
            });
        }

        return affected;
    }


    public async Task<int> DeleteByIdAsync<TKey>(
        IUnitOfWork uow,
        TKey id,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(uow);
        ArgumentNullException.ThrowIfNull(id);

        var dialect = SqlDialectResolver.Resolve(uow.Connection);
        var tableName = GetTableName(dialect);
        var idColumn = dialect.EscapeIdentifier(ID_FIELD);

        var sql = $"DELETE FROM {tableName} WHERE {idColumn} = @Id";

        var cmdDef = new CommandDefinition(
            sql,
            new { Id = id },
            uow.Transaction,
            commandTimeout: dialect.DefaultCommandTimeout,
            cancellationToken: ct);

        var affected = await uow.Connection.ExecuteAsync(cmdDef);
        return affected;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Increments the RowVersion property by 1.
    /// This keeps the entity in sync with the database after a successful update.
    /// </summary>
    private static void IncrementRowVersion(TEntity entity)
    {
        var prop = EntityPropertyCache<TEntity>.Instance.RowVersionProperty;
        if (prop == null) return;

        var value = prop.GetValue(entity);
        if (value == null) return;

        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (!typeof(IConvertible).IsAssignableFrom(type))
            throw new InvalidOperationException(
                $"RowVersion property '{prop.Name}' must be a numeric type (int, long, byte[]), " +
                $"but found '{type.Name}'");

        var nextValue = Convert.ChangeType(
            Convert.ToInt64(value) + 1,
            type);

        prop.SetValue(entity, nextValue);
    }

    private void SetAuditFields(TEntity entity, ChangeAction action)
    {
        var actor = currentUser.GetActor();
        var now = clock.Now;

        if (action == ChangeAction.Insert)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            entity.LogCreatedBy(actor, now);
        }

        entity.LogModifiedBy(actor, now);
    }

    private static string GetTableName(ISqlDialect dialect)
    {
        var tableName = EntityPropertyCache<TEntity>.Instance.TableName;
        var escapedTableName = tableName.Contains('.')
            ? string.Join(".", tableName.Split('.').Select(p => dialect.EscapeIdentifier(p)))
            : dialect.EscapeIdentifier(tableName);
        return escapedTableName;
    }

    #endregion
}
