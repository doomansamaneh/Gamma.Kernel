using Gamma.Kernel.Models;
using Gamma.Kernel.Common;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using {{DomainNamespace}}.Interfaces.{{Schema}};

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Commands;

[RequiresPermission("{{schema_lower}}.{{entity_lower}}.delete")]
public sealed record DeleteBatch{{Entity}}Command(
    List<Guid> Ids
) : BatchActionCommandBase(Ids),
    IAuditableMessage;

public class DeleteBatch{{Entity}}CommandValidator 
    : BatchActionCommandValidatorBase<DeleteBatch{{Entity}}Command>;

public sealed class DeleteBatch{{Entity}}CommandHandler(
    I{{Entity}}Repository repository
) : BatchActionCommandHandlerBase<DeleteBatch{{Entity}}Command>
{
    protected override async ValueTask<Result<int>> ApplyChangeAsync(
        DeleteBatch{{Entity}}Command command,
        CancellationToken ct)
    {
        var affectedRows = await repository.DeleteByIdsAsync(command.Ids, ct);

        if (affectedRows == 0)
        {
            return Result<int>.Fail(ErrorCodes.OperationFailed);
        }

        return Result<int>.Ok(
            affectedRows,
            SuccessCodes.BatchDeleted
        );
    }
}