
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    ValueTask<TEntity> InsertAsync(
        TEntity entity,
        CancellationToken ct = default);

    ValueTask<int> UpdateAsync(
        TEntity entity,
        CancellationToken ct = default);

    ValueTask<int> DeleteByIdAsync<TKey>(
        TKey id,
        CancellationToken ct = default);

    // ValueTask<TEntity?> GetByIdAsync<TKey>(
    //     TKey id,
    //     CancellationToken ct = default);
}

