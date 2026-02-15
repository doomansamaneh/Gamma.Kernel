using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;

namespace Gamma.Next.Application.ProductGroup.Commands;

[RequiresPermission("ast.product-group.update")]
public sealed record UpdateProductGroupCommand(ProductGroupInput ProductGroup, long RowVersion)
    : UpdateCommandBase<Domain.Entities.ProductGroup>,
    IAuditableCommand;
//{
//public override Domain.Entities.ProductGroup GetEntity() => ProductGroup.Adapt<Domain.Entities.ProductGroup>();
//}
