using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;

namespace Gamma.Next.Application.ProductGroup.Commands;

[RequiresPermission("ast.product-group.delete")]
public sealed class DeleteProductGroupCommand
    : DeleteCommandBase,
    IAuditableCommand;