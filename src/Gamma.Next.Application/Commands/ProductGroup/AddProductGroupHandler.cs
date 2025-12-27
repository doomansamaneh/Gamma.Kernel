using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Next.Application.Commands.ProductGroup;

internal class AddProductGroupHandler
    (IRepository<Domain.Entities.ProductGroup> repository,
    IUidGenerator uidGenerator)
    : ICommandHandler<AddProductGroupCommand, Guid>
{
    public async Task<Result<Guid>> Handle(IUnitOfWork uow,
        AddProductGroupCommand command,
         CancellationToken ct = default)
    {
        var entity = new Domain.Entities.ProductGroup
        {
            Id = uidGenerator.New(),
            Code = command.ProductGroup.Code,
            Title = command.ProductGroup.Title,
            Comment = command.ProductGroup.Comment,
            IsActive = command.ProductGroup.IsActive
        };

        await repository.InsertAsync(uow, entity, ct);

        return Result<Guid>.Ok(entity.Id);
    }
}