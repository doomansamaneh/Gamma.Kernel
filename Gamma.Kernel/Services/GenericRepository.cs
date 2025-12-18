using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Caching;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Extensions;

namespace Gamma.Kernel.Services;

public sealed class GenericRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity, new()
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;
    private readonly ICurrentUser _currentUser;
    private readonly ISystemClock _clock;

    public GenericRepository(
        IDbConnection connection,
        ICurrentUser currentUser,
        ISystemClock clock,
        IDbTransaction? transaction = null)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _transaction = transaction;
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    #region CUD Operations

    public async Task<Guid> InsertAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.NormalizeStringProperties();
        SetAuditFields(entity, ChangeAction.Insert);

        var props = EntityPropertyCache<TEntity>.Instance.InsertableProperties;
        var columns = string.Join(",", props.Select(p => p.Name));
        var parameters = string.Join(",", props.Select(p => "@" + p.Name));

        var sql = $"INSERT INTO {EntityPropertyCache<TEntity>.Instance.TableName} ({columns}) VALUES ({parameters})";

        await _connection.ExecuteAsync(sql, entity, _transaction);

        return entity.Id;
    }

    public async Task<int> UpdateAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.NormalizeStringProperties();
        SetAuditFields(entity, ChangeAction.Update);

        var props = EntityPropertyCache<TEntity>.Instance.UpdatableProperties;
        var setClause = string.Join(",", props.Select(p => $"{p.Name} = @{p.Name}"));

        var sql = $"UPDATE {EntityPropertyCache<TEntity>.Instance.TableName} SET {setClause} WHERE Id = @Id";

        // Handle RowVersion for concurrency
        var rowVersionProp = EntityPropertyCache<TEntity>.Instance.RowVersionProperty;
        if (rowVersionProp != null)
        {
            sql += $" AND {rowVersionProp.Name} = @{rowVersionProp.Name}";
        }

        var affected = await _connection.ExecuteAsync(sql, entity, _transaction);
        if (affected == 0) throw new DBConcurrencyException("Entity update failed due to concurrency violation.");

        return affected;
    }

    public async Task<int> DeleteByIdAsync(Guid id)
    {
        var sql = $"DELETE FROM {EntityPropertyCache<TEntity>.Instance.TableName} WHERE Id = @Id";
        var affected = await _connection.ExecuteAsync(sql, new { Id = id }, _transaction);
        return affected;
    }

    #endregion

    #region Helpers

    private void SetAuditFields(TEntity entity, ChangeAction action)
    {
        var user = _currentUser.GetUserName();
        var now = _clock.UtcNow;

        if (action == ChangeAction.Insert)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            entity.SetCreated(user, now);
        }

        entity.SetModified(user, now);
    }

    #endregion
}