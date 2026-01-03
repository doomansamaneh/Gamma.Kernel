using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Commands;

public sealed class GenericCreateCommandHandler<TEntity>(
    IRepository<TEntity> repository,
    IUidGenerator uidGenerator
) : ICreateCommandHandler<TEntity>
    where TEntity : BaseEntity
{
    public async ValueTask<Result<Guid>> Handle(
        GenericCreateCommand<TEntity> command,
        CancellationToken ct)
    {
        if (command.Entity is null)
            throw new ArgumentNullException(nameof(command.Entity));

        var entity = command.Entity;

        // Assign new GUID if needed
        if (entity.Id == Guid.Empty) entity.Id = uidGenerator.New();
        // Insert entity (UoW handled by pipeline)
        await repository.InsertAsync(entity, ct);

        return Result<Guid>.Ok(entity.Id);
    }
}