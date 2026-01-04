using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;

namespace Gamma.Next.Application.Product.Commands;

[RequiresPermission("ast.product.delete")]
public sealed class DeleteProductCommand
    : DeleteCommandBase,
    IAuditableCommand;