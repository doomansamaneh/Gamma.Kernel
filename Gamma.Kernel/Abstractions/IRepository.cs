
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface IRepository_<TEntity>
    where TEntity : BaseEntity
{
    Task<TEntity> InsertAsync(IUnitOfWork uow, TEntity entity, CancellationToken ct = default);
    Task<int> UpdateAsync(IUnitOfWork uow, TEntity entity, CancellationToken ct = default);
    Task<int> DeleteByIdAsync<TKey>(IUnitOfWork uow, TKey id, CancellationToken ct = default);
}

public interface IRepository<TEntity>
where TEntity : BaseEntity
{
    /// <summary>
    /// Inserts the entity into the database and returns the created entity.
    /// </summary>
    ValueTask<TEntity> InsertAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Updates the entity in the database and returns number of affected rows.
    /// </summary>
    ValueTask<int> UpdateAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Deletes the entity by its primary key and returns number of affected rows.
    /// </summary>
    ValueTask<int> DeleteByIdAsync<TKey>(TKey id, CancellationToken ct = default);
}
