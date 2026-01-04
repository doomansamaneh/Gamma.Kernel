using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Next.Application.Product.Commands;

internal sealed class CreateProductCommandHandler(
    IRepository<Domain.Entities.Product> repository
) : ICommandHandler<CreateProductCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateProductCommand command,
        CancellationToken ct)
    {
        if (command?.Product is null)
            return Result<Guid>.Fail("Product cannot be null.");

        var product = await repository.InsertAsync(command.Product.ToEntity(), ct);
        return Result<Guid>.Ok(product.Id);
    }
}


