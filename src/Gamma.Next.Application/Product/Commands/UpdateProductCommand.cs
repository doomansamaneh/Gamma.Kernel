using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Mapster;

namespace Gamma.Next.Application.Product.Commands;


[RequiresPermission("ast.product.update")]
public sealed record UpdateProductCommand(ProductInput Product)
    : UpdateCommandBase<Domain.Entities.Product>,
    IAuditableCommand
{
    public override Domain.Entities.Product GetEntity() => Product.Adapt<Domain.Entities.Product>();
}

