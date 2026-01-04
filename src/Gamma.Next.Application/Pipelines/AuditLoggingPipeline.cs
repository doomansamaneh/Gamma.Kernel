using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Mediator;

namespace Gamma.Next.Application.Pipelines;

internal sealed class AuditLoggingPipeline<TMessage, TResponse>
    (IAuditLogger auditLogger,
    ICurrentUser currentUser,
    ISystemClock systemClock)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IAuditableCommand
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken ct)
    {
        // Call next first (let handler execute)
        var response = await next(message, ct);

        // Log only after successful execution
        await auditLogger.LogAsync(new AuditEntry
        {
            EntityName = "test",
            EntityId = "001",
            Action = Kernel.Enums.AuditAction.Create,
            Actor = currentUser.GetActor(),
            LogTime = systemClock.Now,
            Before = null,
            After = null
        }, ct);

        return response;
    }
}
