using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Next.Application.Product.Commands;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Commands;

internal sealed class DeleteProductGroupCommandHandler(
    IMediator mediator,
    IRepository<Domain.Entities.ProductGroup> repository
) : DeleteCommandHandlerBase<DeleteProductGroupCommand, Domain.Entities.ProductGroup>(repository)
{
    protected override async ValueTask OnBeforeDelete(DeleteProductGroupCommand command, CancellationToken ct)
    {
        await mediator.Send(new DeleteProductCommand { Id = new Guid("019B892B-89C3-7921-8627-CC02A20B4A92") }, ct);
    }
}