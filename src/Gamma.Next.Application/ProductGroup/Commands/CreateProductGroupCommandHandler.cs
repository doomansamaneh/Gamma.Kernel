using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Models;
using Gamma.Next.Application.Product.Commands;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Commands;

internal sealed class CreateProductGroupCommandHandler(
    IMediator mediator,
    IRepository<Domain.Entities.ProductGroup> repository)
    : CreateCommandHandlerBase<CreateProductGroupCommand, Domain.Entities.ProductGroup>(repository)
{
    protected override ValueTask<Domain.Entities.ProductGroup> CreateEntity(CreateProductGroupCommand command, CancellationToken ct)
    {
        var entity = Domain.Entities.ProductGroup.Create(
            command.ProductGroup.Code,
            command.ProductGroup.Title,
            command.ProductGroup.Comment,
            command.ProductGroup.IsActive);
        return ValueTask.FromResult(entity);
    }

    protected override async ValueTask<Result<EmptyUnit>> OnAfterCreate(
            CreateProductGroupCommand command,
            Domain.Entities.ProductGroup entity,
            CancellationToken ct)
    {
        if (command.ProductGroup.Products is not { Count: > 0 })
            return Result<EmptyUnit>.Ok(default);

        foreach (var product in command.ProductGroup.Products)
        {
            product.ProductGroupId = entity.Id;

            var productResult = await mediator.Send(new CreateProductCommand(product), ct);

            if (!productResult.Success)
            {
                return productResult.ToEmptyUnit();
            }
        }

        return Result<EmptyUnit>.Ok(default);
    }
}