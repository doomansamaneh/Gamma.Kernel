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
    #region CUD Operations
    public async Task<TEntity> InsertAsync(IUnitOfWork uow, TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.NormalizeStringProperties();
        SetAuditFields(entity, ChangeAction.Insert);

        var props = EntityPropertyCache<TEntity>.Instance.InsertableProperties;
        var columns = string.Join(",", props.Select(p => p.Name));
        var parameters = string.Join(",", props.Select(p => "@" + p.Name));

        var sql = $"INSERT INTO {EntityPropertyCache<TEntity>.Instance.TableName} ({columns}) VALUES ({parameters})";

        await uow.Connection.ExecuteAsync(sql, entity, uow.Transaction);

        return entity;
    }

    public async Task<int> UpdateAsync(IUnitOfWork uow, TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

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

        var affected = await uow.Connection.ExecuteAsync(sql, entity, uow.Transaction);
        if (affected == 0) throw new DBConcurrencyException("Entity update failed due to concurrency violation.");

        return affected;
    }

    public async Task<int> DeleteByIdAsync(IUnitOfWork uow, Guid id, CancellationToken ct = default)
    {
        var sql = $"DELETE FROM {EntityPropertyCache<TEntity>.Instance.TableName} WHERE Id = @Id";
        var affected = await uow.Connection.ExecuteAsync(sql, new { Id = id }, uow.Transaction);
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

    #endregion
}
