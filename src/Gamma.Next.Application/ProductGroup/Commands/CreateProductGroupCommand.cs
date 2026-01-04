using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;

namespace Gamma.Next.Application.ProductGroup.Commands;

[RequiresPermission("ast.product-group.create")]
public sealed record CreateProductGroupCommand
    : CreateCommandBase<Domain.Entities.ProductGroup>
    , IAuditableCommand
{
   public ProductGroupInput ProductGroup { get; init; } = default!;

   public override Domain.Entities.ProductGroup Entity => ProductGroup.ToEntity();
}