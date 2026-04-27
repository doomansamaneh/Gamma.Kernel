using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract class BatchActionCommandHandlerBase<TCommand>(
) : ICommandHandler<TCommand, Result<int>>
    where TCommand : BatchActionCommandBase
{
    public async ValueTask<Result<int>> Handle(TCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);

        var preResult = await OnBeforeChangeAsync(command, ct);
        if (!preResult.Success)
            return Result<int>.Fail(preResult.Errors, preResult.Message);

        await ApplyChangeAsync(command, ct);

        var postResult = await OnAfterChangeAsync(command, ct);
        if (!postResult.Success)
            return Result<int>.Fail(postResult.Errors, postResult.Message);

        return Result<int>.Ok(command.Ids.Count, "Updated successfully");
    }

    protected abstract ValueTask ApplyChangeAsync(
        TCommand command,
        CancellationToken ct);

    protected virtual ValueTask<Result<EmptyUnit>> OnBeforeChangeAsync(
        TCommand command,
        CancellationToken ct)
        => ValueTask.FromResult(Result<EmptyUnit>.Ok(default));

    protected virtual ValueTask<Result<EmptyUnit>> OnAfterChangeAsync(
        TCommand command,
        CancellationToken ct)
        => ValueTask.FromResult(Result<EmptyUnit>.Ok(default));
}

