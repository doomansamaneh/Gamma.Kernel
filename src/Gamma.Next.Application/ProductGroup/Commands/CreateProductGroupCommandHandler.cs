using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Models;
using Gamma.Next.Application.Product.Commands;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Commands;

internal sealed class CreateProductGroupCommandHandler(
    IMediator mediator,
    IRepository<Domain.Entities.ProductGroup> repository
) : ICommandHandler<CreateProductGroupCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateProductGroupCommand command,
        CancellationToken ct)
    {
        if (command?.ProductGroup is null)
            return Result<Guid>.Fail("ProductGroup cannot be null.");

        var productGroup = await repository.InsertAsync(command.ProductGroup.ToEntity(), ct);
        // Create all products in the group (if any)
        if (command.ProductGroup.Products?.Any() == true)
        {
            foreach (var product in command.ProductGroup.Products)
            {
                product.ProductGroupId = productGroup.Id;
                var productResult = await mediator.Send(new CreateProductCommand(product), ct);

                if (!productResult.Success)
                    return Result<Guid>.Fail(productResult.Errors, productResult.Message);
            }
        }

        return Result<Guid>.Ok(productGroup.Id);
    }
}


