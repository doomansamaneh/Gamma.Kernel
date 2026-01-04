using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;
using Gamma.Kernel.Security;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Commands;

[RequiresPermission("ast.product-group.update")]
public sealed record UpdateProductGroupCommand(ProductGroupInput ProductGroup)
    : IAuditableCommand,
    ICommand<Result<int>>
{
    public Guid Id { get; set; }
    public long RowVersion { get; set; }
}
