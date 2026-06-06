using Gamma.Kernel.Models;
using Gamma.Kernel.Common;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.ProductGroup.Commands;

[RequiresPermission("ast.productgroup.delete")]
public sealed record DeleteBatchProductGroupCommand(
    List<Guid> Ids
) : BatchActionCommandBase(Ids),
    IAuditableMessage;

public class DeleteBatchProductGroupCommandValidator 
    : BatchActionCommandValidatorBase<DeleteBatchProductGroupCommand>;

public sealed class DeleteBatchProductGroupCommandHandler(
    IProductGroupRepository repository
) : BatchActionCommandHandlerBase<DeleteBatchProductGroupCommand>
{
    protected override async ValueTask<Result<int>> ApplyChangeAsync(
        DeleteBatchProductGroupCommand command,
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