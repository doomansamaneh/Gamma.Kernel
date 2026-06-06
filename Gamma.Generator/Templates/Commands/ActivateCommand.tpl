using Gamma.Kernel.Models;
using Gamma.Kernel.Common;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using {{DomainNamespace}}.Interfaces.{{Schema}};

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Commands;

[RequiresPermission("{{schema_lower}}.{{entity_lower}}.activate")]
[RequiresPermission("{{schema_lower}}.{{entity_lower}}.update")]
public sealed record Activate{{Entity}}Command(
    List<Guid> Ids,
    bool IsActive
) : BatchActionCommandBase(Ids),
    IAuditableMessage;

public sealed class Activate{{Entity}}CommandValidator
    : BatchActionCommandValidatorBase<Activate{{Entity}}Command>;

public sealed class Activate{{Entity}}CommandHandler(
    I{{Entity}}Repository repository
) : BatchActionCommandHandlerBase<Activate{{Entity}}Command>
{
    protected override async ValueTask<Result<int>> ApplyChangeAsync(
        Activate{{Entity}}Command command,
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