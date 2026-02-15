using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;

namespace Gamma.Next.Application.Product.Commands;

internal sealed class UpdateProductCommandHandler(IRepository<Domain.Entities.Product> repository)
     : UpdateCommandHandlerBase<UpdateProductCommand, Domain.Entities.Product>(repository)
{
     protected override ValueTask<Domain.Entities.Product> GetEntity(UpdateProductCommand command, CancellationToken ct)
     {
          throw new NotImplementedException();
     }
}