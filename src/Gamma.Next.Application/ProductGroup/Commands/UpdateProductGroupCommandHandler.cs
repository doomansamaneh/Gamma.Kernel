using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Models;
using Gamma.Next.Application.Product.Commands;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Commands;

internal sealed class UpdateProductGroupCommandHandler(
    IMediator mediator,
    IRepository<Domain.Entities.ProductGroup> repository)
    : UpdateCommandHandlerBase<UpdateProductGroupCommand, Domain.Entities.ProductGroup>(repository)
{
    protected override async ValueTask<Result<EmptyUnit>> OnAfterUpdate(
        UpdateProductGroupCommand command,
        int affectedRows,
        CancellationToken ct)
    {
        if (command.ProductGroup.Products is not { Count: > 0 })
            return Result<EmptyUnit>.Ok(default);

        foreach (var product in command.ProductGroup.Products)
        {
            product.ProductGroupId = command.Id;

            //todo: update, delete, insert product based on new list of products
            // var result = await mediator.Send(
            //     product.Id == Guid.Empty
            //         ? new CreateProductCommand(product)
            //         : new UpdateProductCommand(product),
            //     ct);

            // if (!result.Success)
            //     return Result<EmptyUnit>.Fail(result.Errors, result.Message);
        }

        return Result<EmptyUnit>.Ok(default);
    }
}



