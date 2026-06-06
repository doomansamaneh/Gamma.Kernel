using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.Product.Commands;

[RequiresPermission("ast.product.delete")]
public sealed record DeleteProductCommand
    : DeleteCommandBase,
    IAuditableMessage;

internal sealed class DeleteProductCommandHandler(IProductRepository repository
    ) : DeleteCommandHandlerBase<DeleteProductCommand, Domain.Entities.Ast.Product>(repository);
