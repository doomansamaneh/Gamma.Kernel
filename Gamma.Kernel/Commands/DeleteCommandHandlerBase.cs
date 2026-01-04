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
        // Pre-delete hook: can cancel operation by returning a failed Result
        await OnBeforeDelete(command, ct);

        // Main delete operation
        var affected = await repository.DeleteByIdAsync(command.Id, ct);

        if (affected == 0)
            return Result<int>.Fail($"{typeof(TEntity).Name} not found.");

        // Post-delete hook: can log, audit, enqueue events, etc.
        await OnAfterDelete(command, affected, ct);

        return Result<int>.Ok(affected);
    }

    /// <summary>
    /// Called before the delete operation.
    /// Return a failed Result to short-circuit deletion.
    /// Default: allow deletion.
    /// </summary>
    protected virtual ValueTask OnBeforeDelete(
        TCommand command,
        CancellationToken ct
    ) => ValueTask.CompletedTask;

    /// <summary>
    /// Called after the delete operation.
    /// Default: does nothing.
    /// </summary>
    protected virtual ValueTask OnAfterDelete(
        TCommand command,
        int affectedRows,
        CancellationToken ct
    ) => ValueTask.CompletedTask;
}
