
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface IRepository<TEntity>
        where TEntity : BaseEntity, new()
{
    Task<Guid> InsertAsync(TEntity entity);
    Task<int> UpdateAsync(TEntity entity);
    Task<int> DeleteByIdAsync(Guid id);
}
