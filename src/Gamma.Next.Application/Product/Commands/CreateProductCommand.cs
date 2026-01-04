using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;

namespace Gamma.Next.Application.Product.Commands;

[RequiresPermission("ast.product.create")]
public sealed record CreateProductCommand(ProductInput Product)
    : CreateCommandBase<Domain.Entities.Product>
    , IAuditableCommand
{
    public override Domain.Entities.Product Entity => Product.ToEntity();
}

