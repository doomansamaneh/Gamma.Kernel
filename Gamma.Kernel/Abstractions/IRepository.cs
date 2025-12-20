
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface IRepository<TEntity>
    where TEntity : BaseEntity, new()
{
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken ct = default);
    Task<int> UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task<int> DeleteByIdAsync(Guid id, CancellationToken ct = default);
}