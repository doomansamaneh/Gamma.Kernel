using System.Reflection;
using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Exceptions;
using Gamma.Kernel.Security;

namespace Gamma.Kernel.Pipelines;

public sealed class AuthorizationPipeline<TMessage, TResponse>(
    IAuthorizationService authorizationService,
    IAuthorizationContext authContext)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private static readonly bool AllowAnonymous =
        typeof(TMessage).IsDefined(typeof(AllowAnonymousAttribute), true);

    // Cache permissions per message type
    private static readonly string[] Permissions =
        ResolvePermissions();

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken ct)
    {
        // Anonymous endpoint
        if (AllowAnonymous)
        {
            return await next(message, ct);
        }

        // Already authorized
        if (authContext.IsAuthorized)
        {
            return await next(message, ct);
        }

        // No permission metadata
        if (Permissions.Length == 0)
        {
            throw new ForbiddenException(
                $"No permission defined for message: {typeof(TMessage).Name}");
        }

        foreach (var permission in Permissions)
        {
            if (await authorizationService.HasPermissionAsync(permission, ct))
            {
                authContext.MarkAuthorized();

                return await next(message, ct);
            }
        }

        throw new ForbiddenException(
            $"Requires any of permissions [{string.Join(", ", Permissions)}] for message {typeof(TMessage).Name}");
    }

    private static string[] ResolvePermissions()
    {
        return [.. typeof(TMessage)
            .GetCustomAttributes<RequiresPermissionAttribute>(true)
            .Select(x => x.Permission)
            .Distinct(StringComparer.OrdinalIgnoreCase)];
    }
}
