using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Commands;

public class GenericUpdateCommandHandler<TEntity>(IRepository<TEntity> repository)
    : IUpdateCommandHandler<TEntity>
    where TEntity : BaseEntity
{
    public async Task<Result<int>> HandleAsync(
        IUnitOfWork uow,
        GenericUpdateCommand<TEntity> command,
        CancellationToken ct = default)
    {
        var affected = await repository.UpdateAsync(uow, command.Entity, ct);
        return Result<int>.Ok(affected);
    }
}

