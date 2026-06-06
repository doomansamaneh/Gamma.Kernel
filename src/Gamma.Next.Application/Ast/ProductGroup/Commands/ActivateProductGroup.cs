using Gamma.Kernel.Models;
using Gamma.Kernel.Common;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.ProductGroup.Commands;

[RequiresPermission("ast.productgroup.activate")]
[RequiresPermission("ast.productgroup.update")]
public sealed record ActivateProductGroupCommand(
    List<Guid> Ids,
    bool IsActive
) : BatchActionCommandBase(Ids),
    IAuditableMessage;

public sealed class ActivateProductGroupCommandValidator
    : BatchActionCommandValidatorBase<ActivateProductGroupCommand>;

public sealed class ActivateProductGroupCommandHandler(
    IProductGroupRepository repository
) : BatchActionCommandHandlerBase<ActivateProductGroupCommand>
{
    protected override async ValueTask<Result<int>> ApplyChangeAsync(
        ActivateProductGroupCommand command,
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