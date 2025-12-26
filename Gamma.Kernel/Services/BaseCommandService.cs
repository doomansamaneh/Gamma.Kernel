
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Services;

public abstract class BaseCommandService<TAddCommandHandler,
    TAddCommand,
    TEditCommandHandler,
    TEditCommand,
    TDeleteCommandHandler,
    TDeleteCommand>(
        IExecuteHandlerService executeHandlerService,
        TAddCommandHandler addHandler,
        TEditCommandHandler editHandler,
        TDeleteCommandHandler deleteHandler)
    : IApplicationService //BaseDomainCommandService(unitOfWorkFactory)
    where TAddCommand : class
    where TEditCommand : class
    where TDeleteCommand : class
    where TAddCommandHandler : ICommandHandler<TAddCommand, Guid>
    where TEditCommandHandler : ICommandHandler<TEditCommand, int>
    where TDeleteCommandHandler : ICommandHandler<TDeleteCommand, int>
{
    protected readonly TAddCommandHandler _addHandler = addHandler;
    protected readonly TEditCommandHandler _editHandler = editHandler;
    protected readonly TDeleteCommandHandler _deleteHandler = deleteHandler;

    public virtual Task<Result<Guid>> AddAsync(TAddCommand command, CancellationToken ct = default)
        => ExecuteHandlerAsync(uow => _addHandler.Handle(uow, command, ct), ct);

    public virtual Task<Result<int>> EditAsync(TEditCommand command, CancellationToken ct = default)
        => ExecuteHandlerAsync(uow => _editHandler.Handle(uow, command, ct), ct);

    public virtual Task<Result<int>> DeleteAsync(TDeleteCommand command, CancellationToken ct = default)
        => ExecuteHandlerAsync(uow => _deleteHandler.Handle(uow, command, ct), ct);

    protected virtual Task<Result<T>> ExecuteHandlerAsync<T>(Func<IUnitOfWork, Task<Result<T>>> action, CancellationToken ct = default)
    {
        return executeHandlerService.ExecuteHandlerAsync(action, ct);
    }
}