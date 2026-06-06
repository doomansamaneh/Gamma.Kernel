using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract class DeleteCommandHandlerBase<TCommand, TEntity>(
    IRepository<TEntity> repository
) : ICommandHandler<TCommand, Result<int>>
    where TCommand : DeleteCommandBase
    where TEntity : BaseEntity
{
    public async virtual ValueTask<Result<int>> Handle(
        TCommand command,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        // Pre-delete hook
        var preResult = await OnBeforeDelete(command, ct);
        if (!preResult.Success)
            return Result<int>.Fail(preResult.Errors, preResult.Message);

        // Main delete
        var affected = await repository.DeleteByIdAsync(command.Id, ct);

        if (affected == 0)
            return Result<int>.Fail($"{typeof(TEntity).Name} not found.");

        // Post-delete hook
        var postResult = await OnAfterDelete(command, affected, ct);
        if (!postResult.Success)
            return Result<int>.Fail(postResult.Errors, postResult.Message);

        return Result<int>.Ok(affected);
    }

    protected virtual ValueTask<Result<EmptyUnit>> OnBeforeDelete(
        TCommand command,
        CancellationToken ct)
        => ValueTask.FromResult(Result<EmptyUnit>.Ok(default));

    protected virtual ValueTask<Result<EmptyUnit>> OnAfterDelete(
        TCommand command,
        int affectedRows,
        CancellationToken ct)
        => ValueTask.FromResult(Result<EmptyUnit>.Ok(default));
}
