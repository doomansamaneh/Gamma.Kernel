using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;

namespace Gamma.Next.Application.Product.Commands;

internal sealed class CreateProductCommandHandler(IRepository<Domain.Entities.Product> repository)
     : CreateCommandHandlerBase<CreateProductCommand, Domain.Entities.Product>(repository);

