using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Commands;

internal sealed class UpdateProductGroupCommandHandler(
    IMediator mediator,
    IRepository<Domain.Entities.ProductGroup> repository)
    : UpdateCommandHandlerBase<UpdateProductGroupCommand, Domain.Entities.ProductGroup>(repository)
{
    protected override ValueTask<Domain.Entities.ProductGroup> GetEntity(UpdateProductGroupCommand command, CancellationToken ct)
    {
        var entity = Domain.Entities.ProductGroup.Create(
            command.ProductGroup.Code,
            command.ProductGroup.Title,
            command.ProductGroup.Comment,
            command.ProductGroup.IsActive);
        entity.Id = command.Id;
        entity.RowVersion = command.RowVersion;
        return ValueTask.FromResult(entity);
    }

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



