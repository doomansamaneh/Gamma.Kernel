using Gamma.Kernel.Models;
using Gamma.Kernel.Common;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.Product.Commands;

[RequiresPermission("ast.product.activate")]
[RequiresPermission("ast.product.update")]
public sealed record ActivateProductCommand(
    List<Guid> Ids,
    bool IsActive
) : BatchActionCommandBase(Ids),
    IAuditableMessage;

public sealed class ActivateProductCommandValidator
    : BatchActionCommandValidatorBase<ActivateProductCommand>;

public sealed class ActivateProductCommandHandler(
    IProductRepository repository
) : BatchActionCommandHandlerBase<ActivateProductCommand>
{
    protected override async ValueTask<Result<int>> ApplyChangeAsync(
        ActivateProductCommand command,
        CancellationToken ct)
    {
        var affectedRows = await repository.UpdateDynamicByIdsAsync(command.Ids, new { command.IsActive }, ct);
        return Result<int>.Ok(
            affectedRows,
            command.IsActive
                ? SuccessCodes.BatchActivated
                : SuccessCodes.BatchDeactivated);
    }
}