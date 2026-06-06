using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.ProductGroup.Commands;

[RequiresPermission("ast.productgroup.delete")]
public sealed record DeleteProductGroupCommand
    : DeleteCommandBase,
    IAuditableMessage;

internal sealed class DeleteProductGroupCommandHandler(IProductGroupRepository repository
    ) : DeleteCommandHandlerBase<DeleteProductGroupCommand, Domain.Entities.Ast.ProductGroup>(repository);
