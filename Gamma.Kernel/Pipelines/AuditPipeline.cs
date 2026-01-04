using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Logging;
using Gamma.Kernel.Persistence;
using Mediator;

namespace Gamma.Kernel.Pipelines;

public sealed class AuditPipeline<TMessage, TResponse>(
    IAuditLogger auditLogger,
    IAuditMetadataResolver resolver,
    ICurrentUser currentUser,
    ISystemClock clock)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IAuditableCommand
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken ct)
    {
        using var scope =
            !AuditableScope.HasCurrent
                ? AuditableScope.Create()
                : null;

        var response = await next(message, ct);

        if (!UnitOfWorkScope.HasCurrent)
            return response;

        var auditEntry = resolver.Resolve(message);
        if (auditEntry != null)
        {
            auditEntry.CorrelationId = AuditableScope.Current;
            auditEntry.Actor = currentUser.GetActor();
            auditEntry.LogTime = clock.Now;
        }

        UnitOfWorkScope.Current.OnCommitted(async ct =>
        {
            if (auditEntry != null) await auditLogger.LogAsync(auditEntry, ct);
        });

        return response;
    }
}

