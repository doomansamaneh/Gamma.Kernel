using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Behaviors;

public sealed class AuditingCommandHandlerDecorator<TCommand, TResult>(
    ICommandHandler_<TCommand, TResult> inner,
    IAuditLogger audit,
    ICurrentUser currentUser,
    ISystemClock systemClock)
    : ICommandHandler_<TCommand, TResult>
{
    public async Task<Result<TResult>> HandleAsync(
        IUnitOfWork uow,
        TCommand command,
        CancellationToken ct)
    {
        var result = await inner.HandleAsync(uow, command, ct);
        //Audit ONLY on success
        if (!result.Success) return result;
        if (command is not IAuditableCommand auditable)
            return result;

        uow.OnCommitted(async ct =>
         {
             await audit.LogAsync(new AuditEntry
             {
                 EntityName = auditable.EntityName,
                 EntityId = auditable.EntityId,
                 Action = auditable.Action,
                 Actor = currentUser.GetActor(),
                 LogTime = systemClock.Now,
                 Before = auditable.Before,
                 After = auditable.After
             }, ct);
         });

        return result;
    }
}
