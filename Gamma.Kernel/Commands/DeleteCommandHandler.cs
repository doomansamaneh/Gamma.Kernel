using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Commands;

public class DeleteCommandHandler<TEntity, TKey>(IRepository<TEntity> repository)
    : IDeleteCommandHandler<TEntity, TKey>
    where TEntity : BaseEntity
{
    public async Task<Result<int>> Handle(
        IUnitOfWork uow,
        GenericDeleteCommand<TEntity, TKey> command,
        CancellationToken ct = default)
    {
        var affected = await repository.DeleteByIdAsync(uow, command.Id, ct);
        return Result<int>.Ok(affected);
    }
}
