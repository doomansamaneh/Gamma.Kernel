using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract class CreateCommandHandlerBase<TCommand, TEntity>(
    IRepository<TEntity> repository
) : ICommandHandler<TCommand, Result<Guid>>
    where TCommand : CreateCommandBase<TEntity>
    where TEntity : BaseEntity
{
    protected IRepository<TEntity> Repository { get; } = repository;

    public async virtual ValueTask<Result<Guid>> Handle(
        TCommand command,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);

        // Pre-create hook: can return failure to short-circuit creation
        var preResult = await OnBeforeCreate(command, ct);
        if (!preResult.Success)
            return Result<Guid>.Fail(preResult.Errors, preResult.Message);

        // Main create operation
        var entity = await Repository.InsertAsync(command.GetEntity(), ct);

        // Post-create hook: logging, domain events, nested entity creation
        var postResult = await OnAfterCreate(command, entity, ct);
        if (!postResult.Success)
            return Result<Guid>.Fail(postResult.Errors, postResult.Message);

        return Result<Guid>.Ok(entity.Id);
    }

    protected virtual ValueTask<Result<EmptyUnit>> OnBeforeCreate(
        TCommand command,
        CancellationToken ct) => ValueTask.FromResult(Result<EmptyUnit>.Ok(default));

    protected virtual ValueTask<Result<EmptyUnit>> OnAfterCreate(
        TCommand command,
        TEntity entity,
        CancellationToken ct) => ValueTask.FromResult(Result<EmptyUnit>.Ok(default));
}


