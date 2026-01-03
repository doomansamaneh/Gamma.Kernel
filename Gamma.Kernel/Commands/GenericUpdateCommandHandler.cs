using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Commands;

public sealed class GenericUpdateCommandHandler<TEntity>(
    IRepository<TEntity> repository
) : IUpdateCommandHandler<TEntity>
    where TEntity : BaseEntity
{
    public async ValueTask<Result<int>> Handle(
        GenericUpdateCommand<TEntity> command,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(command.Entity, nameof(command.Entity));

        var affected = await repository.UpdateAsync(command.Entity, ct);
        return Result<int>.Ok(affected);
    }
}


