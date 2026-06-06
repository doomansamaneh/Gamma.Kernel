using Gamma.Kernel.Entities;
using Gamma.Kernel.Dapper;

namespace Gamma.Kernel.Abstractions;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    ValueTask<TEntity> InsertAsync(
        TEntity entity,
        CancellationToken ct = default);

    ValueTask<IEnumerable<TEntity>> InsertBatchAsync(
        IReadOnlyCollection<TEntity> entities,
        CancellationToken ct = default);

    ValueTask<int> UpdateAsync(
        TEntity entity,
        CancellationToken ct = default);

    ValueTask<int> UpdateDynamicAsync(
        Guid id,
        object updatedFields,
        CancellationToken ct = default);

    ValueTask<int> UpdateDynamicByIdsAsync(
        IEnumerable<Guid> ids,
        object updatedFields,
        CancellationToken ct = default);

    ValueTask<int> UpdateDynamicWhereAsync(
        object updatedFields,
        object whereConditions,
        CancellationToken ct = default);

    ValueTask<int> DeleteByIdAsync<TKey>(
        TKey id,
        CancellationToken ct = default);

    ValueTask<int> DeleteByIdsAsync<TKey>(
        IEnumerable<TKey> ids,
        CancellationToken ct = default);

    ValueTask<int> DeleteDynamicAsync(
        object whereConditions,
        CancellationToken ct = default);

    ValueTask<TEntity?> GetByIdAsync<TKey>(
        TKey id,
        CancellationToken ct = default);

    ValueTask<int> ExecuteAsync(
        SqlBuilder query,
        CancellationToken ct = default);
}

