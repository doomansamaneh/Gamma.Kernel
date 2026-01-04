using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;

namespace Gamma.Next.Application.Product.Commands;

internal sealed class DeleteProductCommandHandler(IRepository<Domain.Entities.Product> repository)
    : DeleteCommandHandlerBase<DeleteProductCommand, Domain.Entities.Product>(repository);

