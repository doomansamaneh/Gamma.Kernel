
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    Task<TEntity> InsertAsync(IUnitOfWork uow, TEntity entity, CancellationToken ct = default);
    Task<int> UpdateAsync(IUnitOfWork uow, TEntity entity, CancellationToken ct = default);
    Task<int> DeleteByIdAsync<TKey>(IUnitOfWork uow, TKey id, CancellationToken ct = default);
}