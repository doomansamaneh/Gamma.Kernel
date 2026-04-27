using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract class MergeCommandHandlerBase<TCommand, TEntity>(
    IRepository<TEntity> repository
) : ICommandHandler<TCommand, Result<int>>
    where TCommand : MergeCommandBase
    where TEntity : BaseEntity
{
    public async ValueTask<Result<int>> Handle(TCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);

        var entity = await repository.GetByIdAsync(command.TargetId, ct);
        if (entity == null)
            return Result<int>.Fail($"{typeof(TEntity).Name} not found.");

        await UpdateRelatedEntitiesAsync(command, entity, ct);

        if (command.DeleteSource)
        {
            var preDeleteResult = await OnBeforeDeleteSourcesAsync(command, ct);
            if (!preDeleteResult.Success)
                return Result<int>.Fail(preDeleteResult.Errors, preDeleteResult.Message);

            foreach (var sourceId in command.SourceIds)
            {
                await repository.DeleteByIdAsync(sourceId, ct);
            }
        }

        return Result<int>.Ok(command.SourceIds.Count, "Merged successfully");
    }

    protected abstract ValueTask UpdateRelatedEntitiesAsync(
        TCommand command,
        TEntity target,
        CancellationToken ct);

    protected virtual ValueTask<Result<EmptyUnit>> OnBeforeDeleteSourcesAsync(
        TCommand command,
        CancellationToken ct)
        => ValueTask.FromResult(Result<EmptyUnit>.Ok(default));
}