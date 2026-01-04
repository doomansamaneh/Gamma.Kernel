using System.Reflection;
using System.Runtime.CompilerServices;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Exceptions;
using Gamma.Kernel.Security;
using Mediator;

namespace Gamma.Next.Application.Pipelines;

internal sealed class AuthorizationPipeline<TMessage, TResponse>(
    IAuthorizationService authorizationService,
    IAuthorizationContext authContext
) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    // Cache permissions per message type
    private static readonly string[]? CachedPermissions = ResolvePermissions();

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken ct)
    {
        // Already authorized → skip
        if (authContext.IsAuthorized)
            return await next(message, ct);

        if (CachedPermissions != null && CachedPermissions.Length > 0)
        {
            foreach (var permission in CachedPermissions)
            {
                if (await authorizationService.HasPermissionAsync(permission, ct))
                {
                    authContext.MarkAuthorized();
                    return await next(message, ct);
                }
            }

            throw new ForbiddenException($"Requires any of the following permissions: {typeof(TMessage).Name} ({string.Join(", ", CachedPermissions)})");
        }

        throw new ForbiddenException($"No permission attribute defined on this message: {typeof(TMessage).Name}");
    }

    private static string[]? ResolvePermissions()
    {
        var attrs = typeof(TMessage)
            .GetCustomAttributes<RequiresPermissionAttribute>(true)
            .ToArray();

        if (attrs.Length > 0)
            return attrs.Select(a => a.Permission).ToArray();

        // fallback for single attribute
        var single = typeof(TMessage)
            .GetCustomAttribute<RequiresPermissionAttribute>(true);

        return single != null ? [single.Permission] : null;
    }
}

