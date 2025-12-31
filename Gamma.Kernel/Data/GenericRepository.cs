using System.Data;
using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Caching;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Extensions;

namespace Gamma.Kernel.Data;

public sealed class GenericRepository<TEntity>(
    ICurrentUser currentUser,
    ISystemClock clock) : IRepository<TEntity>
    where TEntity : BaseEntity, new()
{
    private const string ID_FIELD = "Id";
    #region CUD Operations
    public async Task<TEntity> InsertAsync(IUnitOfWork uow, TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.NormalizeStringProperties();
        SetAuditFields(entity, ChangeAction.Insert);

        var dialect = SqlDialectResolver.Resolve(uow.Connection);
        var cache = EntityPropertyCache<TEntity>.Instance;
        var props = cache.InsertableProperties;
        var columns = string.Join(",", props.Select(p => dialect.EscapeIdentifier(p.Name)));
        var parameters = string.Join(",", props.Select(p => "@" + p.Name));

        var tableName = GetTableName(dialect);
        var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

        if (cache.IdentityProperty is not null)
        {
            sql = $"{sql} {dialect.GetLastInsertIdSql()}";
            var id = await uow.Connection.ExecuteScalarAsync(sql, entity, uow.Transaction, dialect.DefaultCommandTimeout);
            entity.SetPropertyValue(cache.IdentityProperty.Name, id);
            //cache.IdentityProperty.SetValue(entity, Convert.ChangeType(id, cache.IdentityProperty.PropertyType));
        }
        else
        {
            await uow.Connection.ExecuteAsync(sql, entity, uow.Transaction, dialect.DefaultCommandTimeout);
        }

        return entity;
    }

    public async Task<int> UpdateAsync(IUnitOfWork uow, TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.NormalizeStringProperties();
        SetAuditFields(entity, ChangeAction.Update);

        var dialect = SqlDialectResolver.Resolve(uow.Connection);
        var props = EntityPropertyCache<TEntity>.Instance.UpdatableProperties;
        var setClause = string.Join(",", props.Select(p => $"{dialect.EscapeIdentifier(p.Name)} = @{p.Name}"));

        var tableName = GetTableName(dialect);
        var sql = $"UPDATE {tableName} SET {setClause} WHERE {dialect.EscapeIdentifier(ID_FIELD)} = @Id";

        // Handle RowVersion for concurrency
        var rowVersionProp = EntityPropertyCache<TEntity>.Instance.RowVersionProperty;
        if (rowVersionProp != null)
        {
            sql += $" AND {dialect.EscapeIdentifier(rowVersionProp.Name)} = @{rowVersionProp.Name}";
        }

        var affected = await uow.Connection.ExecuteAsync(sql, entity, uow.Transaction, dialect.DefaultCommandTimeout);
        if (affected == 0) throw new DBConcurrencyException("Entity update failed due to concurrency violation.");

        return affected;
    }

    public async Task<int> DeleteByIdAsync<TKey>(IUnitOfWork uow, TKey id, CancellationToken ct = default)
    {
        var dialect = SqlDialectResolver.Resolve(uow.Connection);
        var tableName = GetTableName(dialect);
        var sql = $"DELETE FROM {tableName} WHERE ${dialect.EscapeIdentifier(ID_FIELD)} = @Id";
        var affected = await uow.Connection.ExecuteAsync(sql, new { Id = id }, uow.Transaction, dialect.DefaultCommandTimeout);
        return affected;
    }

    #endregion

    #region Helpers

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
