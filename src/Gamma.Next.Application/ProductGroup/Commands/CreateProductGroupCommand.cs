using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Models;
using Gamma.Kernel.Security;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Commands;

[RequiresPermission("ast.product-group.create")]
public sealed record CreateProductGroupCommand(ProductGroupInput ProductGroup)
     : IAuditableCommand,
        ICommand<Result<Guid>>;