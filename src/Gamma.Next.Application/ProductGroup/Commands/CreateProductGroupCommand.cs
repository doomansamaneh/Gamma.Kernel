using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Mapster;

namespace Gamma.Next.Application.ProductGroup.Commands;

[RequiresPermission("ast.product-group.create")]
public sealed record CreateProductGroupCommand(ProductGroupInput ProductGroup)
    : CreateCommandBase<Domain.Entities.ProductGroup>,
    IAuditableCommand
{
    public override Domain.Entities.ProductGroup Entity => ProductGroup.Adapt<Domain.Entities.ProductGroup>();
}