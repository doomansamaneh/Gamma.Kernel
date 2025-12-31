using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Commands;

public class CreateCommandHandler<TEntity>(IRepository<TEntity> repository, IUidGenerator uidGenerator)
    : ICreateCommandHandler<TEntity>
    where TEntity : BaseEntity
{
    public async Task<Result<Guid>> HandleAsync(IUnitOfWork uow, GenericCreateCommand<TEntity> command, CancellationToken ct = default)
    {
        var entity = command.Entity;

        if (entity.Id == Guid.Empty) entity.Id = uidGenerator.New();

        await repository.InsertAsync(uow, entity, ct);

        return Result<Guid>.Ok(entity.Id);
    }
}
