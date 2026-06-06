using System.Data;
using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Caching;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Persistence;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Exceptions;

namespace Gamma.Kernel.Data;

public abstract class GenericRepository<TEntity>(
    ICurrentUser currentUser,
    IUidGenerator uidGenerator,
    ISystemClock clock
) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected const int BATCH_SIZE = 1000;
    private const string ID_FIELD = "Id";

    protected static IUnitOfWork Uow => UnitOfWorkScope.Current;
    protected ISqlDialect Dialect => SqlDialectResolver.Resolve(Uow.Connection);
    protected ISystemClock Clock => clock;
    protected ICurrentUser CuurentUser => currentUser;
    protected IUidGenerator UidGenerator => uidGenerator;

    #region CUD Operations

    public async ValueTask<TEntity> InsertAsync(
        TEntity entity,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        PrepareEntityForInsert(entity);

        var dialect = Dialect;
        var cache = EntityPropertyCache<TEntity>.Instance;
        var sql = BuildInsertSql(dialect);

        if (cache.IdentityProperty is not null)
        {
            sql += $" {dialect.GetLastInsertIdSql()}";

            var tx = Uow.BeginTransactionIfNeeded();

            var id = await Uow.Connection.ExecuteScalarAsync(
                new CommandDefinition(
                    dialect.EscapeSql(sql),
                    entity,
                    tx,
                    dialect.DefaultCommandTimeout,
                    cancellationToken: ct));

            if (id != null && id != DBNull.Value)
                entity.SetPropertyValue(cache.IdentityProperty.Name, Convert.ChangeType(id, cache.IdentityProperty.PropertyType));
        }
        else
        {
            await ExecuteAsync(sql, entity, dialect, ct);
        }

        return entity;
    }

    public async ValueTask<IEnumerable<TEntity>> InsertBatchAsync(
        IReadOnlyCollection<TEntity> entities,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var list = entities as IList<TEntity> ?? entities.ToList();

        if (list.Count == 0)
            return list;

        foreach (var entity in list)
        {
            PrepareEntityForInsert(entity);
        }

        var dialect = Dialect;
        var cache = EntityPropertyCache<TEntity>.Instance;

        if (cache.IdentityProperty is not null)
            throw new NotSupportedException("Batch insert with identity columns is not supported.");

        var sql = BuildInsertSql(dialect);

        foreach (var batch in list.Chunk(BATCH_SIZE))
        {
            await ExecuteAsync(sql, batch, dialect, ct);
        }

        return list;
    }

    public async ValueTask<int> UpdateAsync(
        TEntity entity,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.NormalizeStringProperties();
        SetAuditFields(entity, ChangeAction.Update);

        var dialect = Dialect;
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

        var affected = await ExecuteAsync(sql, entity, dialect, ct);

        if (affected == 0 && rowVersionProp != null)
            throw new DatabaseException(DatabaseErrorType.ConcurrencyConflict,
                $"Entity update failed due to concurrency violation. Type: {typeof(TEntity).Name}, Id: {entity.Id}");

        if (rowVersionProp != null)
            IncrementRowVersion(entity);

        return affected;
    }

    public async ValueTask<int> UpdateDynamicAsync(
        Guid id,
        object updatedFields,
        CancellationToken ct = default)
    {
        return await UpdateDynamicWhereAsync(updatedFields, new { id }, ct);
    }

    public async ValueTask<int> UpdateDynamicWhereAsync(
        object updatedFields,
        object whereConditions,
        CancellationToken ct = default)
    {
        var whereDict = whereConditions.ToDictionary();
        if (whereDict == null || whereDict.Count == 0)
            throw new ArgumentException("No WHERE conditions specified", nameof(whereConditions));

        var dialect = Dialect;
        var (setClause, parameters, hasRowVersion) = PrepareDynamicUpdate(updatedFields, dialect);

        var whereParts = new List<string>();
        foreach (var kvp in whereDict)
        {
            var column = dialect.EscapeIdentifier(kvp.Key);
            var paramName = $"where_{kvp.Key}";

            whereParts.Add($"{column} = @{paramName}");
            parameters.Add(paramName, kvp.Value);
        }

        var whereClause = string.Join(" AND ", whereParts);
        var tableName = GetTableName(dialect);
        var sql = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

        var affected = await ExecuteAsync(sql, parameters, dialect, ct);

        if (affected == 0 &&
            hasRowVersion &&
            whereDict.Keys.Any(k =>
                string.Equals(k, EntityPropertyCache<TEntity>.Instance.RowVersionProperty!.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DatabaseException(DatabaseErrorType.ConcurrencyConflict,
                $"Entity update failed due to concurrency violation. Type: {typeof(TEntity).Name}");
        }

        return affected;
    }

    public async ValueTask<int> UpdateDynamicByIdsAsync(
        IEnumerable<Guid> ids,
        object updatedFields,
        CancellationToken ct = default)
    {
        var idList = ids?.AsList();
        if (idList == null || idList.Count == 0)
            return 0;

        var dialect = Dialect;
        var (setClause, parameters, _) = PrepareDynamicUpdate(updatedFields, dialect);

        var idColumn = dialect.EscapeIdentifier(ID_FIELD);
        parameters.Add("Ids", idList);

        var tableName = GetTableName(dialect);
        var sql = $"UPDATE {tableName} SET {setClause} WHERE {idColumn} IN @Ids";

        return await ExecuteAsync(sql, parameters, dialect, ct);
    }

    public async ValueTask<int> ExecuteAsync(
        SqlBuilder query,
        CancellationToken ct = default)
    {
        var dialect = Dialect;
        var sql = query.ToSqlString(dialect);

        ArgumentException.ThrowIfNullOrWhiteSpace(sql);

        return await ExecuteAsync(sql, query.Parameters, dialect, ct);
    }

    public async ValueTask<int> DeleteByIdAsync<TKey>(
        TKey id,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        var dialect = Dialect;
        var tableName = GetTableName(dialect);
        var idColumn = dialect.EscapeIdentifier(ID_FIELD);

        var sql = $"DELETE FROM {tableName} WHERE {idColumn} = @Id";

        return await ExecuteAsync(sql, new { Id = id }, dialect, ct);
    }

    public async ValueTask<int> DeleteByIdsAsync<TKey>(
        IEnumerable<TKey> ids,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(ids);

        var idList = ids.ToList();
        if (idList.Count == 0)
            return 0;

        var dialect = Dialect;
        var tableName = GetTableName(dialect);
        var idColumn = dialect.EscapeIdentifier(ID_FIELD);

        var sql = $"DELETE FROM {tableName} WHERE {idColumn} IN @Ids";

        return await ExecuteAsync(sql, new { Ids = idList }, dialect, ct);
    }

    public async ValueTask<int> DeleteDynamicAsync(
    object whereConditions,
    CancellationToken ct = default)
    {
        var whereDict = whereConditions.ToDictionary();
        if (whereDict == null || whereDict.Count == 0)
            throw new ArgumentException("No WHERE conditions specified", nameof(whereConditions));

        var dialect = Dialect;

        var whereParts = new List<string>();
        var parameters = new DynamicParameters();

        foreach (var kvp in whereDict)
        {
            var column = dialect.EscapeIdentifier(kvp.Key);
            var paramName = $"where_{kvp.Key}";
            parameters.Add(paramName, kvp.Value);

            if (kvp.Value is IEnumerable<object> enumerable && kvp.Value is not string)
            {
                whereParts.Add($"{column} IN @{paramName}");
            }
            else
            {
                whereParts.Add($"{column} = @{paramName}");
            }
        }

        var whereClause = string.Join(" AND ", whereParts);
        var tableName = GetTableName(dialect);
        var sql = $"DELETE FROM {tableName} WHERE {whereClause}";

        var affected = await ExecuteAsync(sql, parameters, dialect, ct);
        return affected;
    }

    public virtual async ValueTask<TEntity?> GetByIdAsync<TKey>(
        TKey id,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        var dialect = Dialect;
        var tableName = GetTableName(dialect);
        var idColumn = dialect.EscapeIdentifier(ID_FIELD);

        var sql = $"SELECT * FROM {tableName} WHERE {idColumn} = @Id";

        return await Uow.Connection.QueryFirstOrDefaultAsync<TEntity>(
            sql,
            new { Id = id },
            Uow.Transaction,
            commandTimeout: dialect.DefaultCommandTimeout);
    }

    #endregion

    #region Helpers

    private void PrepareEntityForInsert(TEntity entity)
    {
        entity.NormalizeStringProperties();

        if (entity.Id == Guid.Empty)
            entity.Id = uidGenerator.New();

        SetAuditFields(entity, ChangeAction.Insert);
    }

    private static string BuildInsertSql(ISqlDialect dialect)
    {
        var props = EntityPropertyCache<TEntity>.Instance.InsertableProperties;
        var columns = string.Join(", ", props.Select(p => dialect.EscapeIdentifier(p.Name)));
        var parameters = string.Join(", ", props.Select(p => "@" + p.Name));
        var tableName = GetTableName(dialect);

        return $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";
    }

    private (string SetClause, DynamicParameters Parameters, bool HasRowVersion) PrepareDynamicUpdate(
        object updatedFields,
        ISqlDialect dialect)
    {
        updatedFields.NormalizeStringProperties();

        var updatedDict = updatedFields.ToDictionary();
        if (updatedDict == null || updatedDict.Count == 0)
            throw new ArgumentException("No fields to update.", nameof(updatedFields));

        var cache = EntityPropertyCache<TEntity>.Instance;
        var rowVersionProp = cache.RowVersionProperty;
        var hasRowVersion = rowVersionProp != null;

        var allowed = cache.UpdatableProperties
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        updatedDict = updatedDict
            .Where(x => allowed.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value);

        AppendAuditFieldsToDictionary(updatedDict);

        var parameters = new DynamicParameters();
        var setParts = new List<string>();

        foreach (var kvp in updatedDict)
        {
            var column = dialect.EscapeIdentifier(kvp.Key);
            var paramName = $"set_{kvp.Key}";

            setParts.Add($"{column} = @{paramName}");
            parameters.Add(paramName, kvp.Value);
        }

        if (hasRowVersion)
        {
            var col = dialect.EscapeIdentifier(rowVersionProp!.Name);
            setParts.Add($"{col} = {col} + 1");
        }

        var setClause = string.Join(", ", setParts);
        return (setClause, parameters, hasRowVersion);
    }

    protected async ValueTask<int> ExecuteAsync(
        string sql,
        object? parameters,
        ISqlDialect dialect,
        CancellationToken ct = default)
    {
        var tx = Uow.BeginTransactionIfNeeded();

        return await Uow.Connection.ExecuteAsync(
            new CommandDefinition(
                dialect.EscapeSql(sql),
                parameters,
                tx,
                dialect.DefaultCommandTimeout,
                cancellationToken: ct));
    }

    protected async ValueTask<int> ExecuteAsync(
        string sql,
        object? parameters,
        CancellationToken ct = default)
    {
        return await ExecuteAsync(sql, parameters, Dialect, ct);
    }

    private void AppendAuditFieldsToDictionary(IDictionary<string, object> dict)
    {
        var actor = currentUser.GetActor();
        var now = clock.Now;

        var cache = EntityPropertyCache<TEntity>.Instance;
        var props = cache.UpdatableProperties.Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (props.Contains(Constants.LOG_MODIFIED_BY_FIELD))
            dict[Constants.LOG_MODIFIED_BY_FIELD] = actor.Serialize();

        if (props.Contains(Constants.LOG_DATE_MODIFIED_FIELD))
            dict[Constants.LOG_DATE_MODIFIED_FIELD] = now;
    }

    private static void IncrementRowVersion(TEntity entity)
    {
        var prop = EntityPropertyCache<TEntity>.Instance.RowVersionProperty;
        if (prop == null) return;

        var value = prop.GetValue(entity);
        if (value == null) return;

        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (!typeof(IConvertible).IsAssignableFrom(type))
            throw new InvalidOperationException(
                $"RowVersion property '{prop.Name}' must be numeric but found '{type.Name}'");

        var nextValue = Convert.ChangeType(Convert.ToInt64(value) + 1, type);
        prop.SetValue(entity, nextValue);
    }

    protected void SetAuditFields(TEntity entity, ChangeAction action)
    {
        var actor = currentUser.GetActor();
        var now = clock.Now;

        if (action == ChangeAction.Insert)
            entity.LogCreatedBy(actor, now);

        entity.LogModifiedBy(actor, now);
    }

    protected static string GetTableName(ISqlDialect dialect)
    {
        var tableName = EntityPropertyCache<TEntity>.Instance.TableName;
        return dialect.EscapeIdentifier(tableName);
    }

    #endregion
}