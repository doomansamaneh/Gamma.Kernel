using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract class UpdateCommandHandlerBase<TCommand, TEntity>(
    IRepository<TEntity> repository
) : ICommandHandler<TCommand, Result<int>>
    where TCommand : UpdateCommandBase<TEntity>
    where TEntity : BaseEntity
{
    protected IRepository<TEntity> Repository { get; } = repository;

    public async virtual ValueTask<Result<int>> Handle(
        TCommand command,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);

        var entity = await Repository.GetByIdAsync(command.Id, ct);
        if (entity == null) return Result<int>.Fail($"{typeof(TEntity).Name} not found.");

        var preResult = await OnBeforeUpdate(command, entity, ct);
        if (!preResult.Success)
            return Result<int>.Fail(preResult.Errors, preResult.Message);

        await UpdateEntity(command, entity, ct);
        var affected = await Repository.UpdateAsync(entity, ct);
        if (affected == 0)
            return Result<int>.Fail($"{typeof(TEntity).Name} not found.");

        var postResult = await OnAfterUpdate(command, entity, affected, ct);
        if (!postResult.Success)
            return Result<int>.Fail(postResult.Errors, postResult.Message);

        return Result<int>.Ok(affected);
    }

    protected abstract ValueTask UpdateEntity(
       TCommand command,
       TEntity entity,
       CancellationToken ct);

    protected virtual ValueTask<Result<EmptyUnit>> OnBeforeUpdate(
        TCommand command,
        TEntity entity,
        CancellationToken ct)
        => ValueTask.FromResult(Result<EmptyUnit>.Ok(default));

    protected virtual ValueTask<Result<EmptyUnit>> OnAfterUpdate(
        TCommand command,
        TEntity entity,
        int affectedRows,
        CancellationToken ct)
        => ValueTask.FromResult(Result<EmptyUnit>.Ok(default));
}
