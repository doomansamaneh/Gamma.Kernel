using Gamma.Kernel.Models;
using Gamma.Kernel.Common;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.Product.Commands;

[RequiresPermission("ast.product.delete")]
public sealed record DeleteBatchProductCommand(
    List<Guid> Ids
) : BatchActionCommandBase(Ids),
    IAuditableMessage;

public class DeleteBatchProductCommandValidator 
    : BatchActionCommandValidatorBase<DeleteBatchProductCommand>;

public sealed class DeleteBatchProductCommandHandler(
    IProductRepository repository
) : BatchActionCommandHandlerBase<DeleteBatchProductCommand>
{
    protected override async ValueTask<Result<int>> ApplyChangeAsync(
        DeleteBatchProductCommand command,
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