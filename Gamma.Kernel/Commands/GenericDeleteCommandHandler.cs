using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Commands;

public sealed class GenericDeleteCommandHandler<TEntity, TKey>(
    IRepository<TEntity> repository
) : IDeleteCommandHandler<TEntity, TKey>
    where TEntity : BaseEntity
{
    public async ValueTask<Result<int>> Handle(
        GenericDeleteCommand<TEntity, TKey> command,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        var affected = await repository.DeleteByIdAsync(command.Id, ct);
        return Result<int>.Ok(affected);
    }
}