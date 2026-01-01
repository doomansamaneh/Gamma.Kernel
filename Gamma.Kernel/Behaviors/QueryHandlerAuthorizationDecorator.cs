
using System.Reflection;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Exceptions;
using Gamma.Kernel.Security;

namespace Gamma.Kernel.Behaviors;

public sealed class QueryHandlerAuthorizationDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> inner,
    IAuthorizationService authorizationService)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private static readonly IReadOnlyList<string>? CachedPermissions = ResolvePermissions();

    public async Task<TResponse> HandleAsync(
        TQuery query,
        CancellationToken ct = default)
    {
        if (CachedPermissions is not null && CachedPermissions.Count > 0)
        {
            foreach (var permission in CachedPermissions)
            {
                if (await authorizationService.HasPermissionAsync(permission, ct))
                    return await inner.HandleAsync(query, ct);
            }

            throw new ForbiddenException($"Requires any of: {string.Join(", ", CachedPermissions)}");
        }

        return await inner.HandleAsync(query, ct);
    }

    private static string[]? ResolvePermissions()
    {
        var attrs = typeof(TQuery)
            .GetCustomAttributes<RequiresPermissionAttribute>(true)
            .ToArray();

        if (attrs.Length > 0) return [.. attrs.Select(a => a.Permission)];

        // fallback to single-permission attribute if you keep it
        var single = typeof(TQuery)
            .GetCustomAttribute<RequiresPermissionAttribute>(true);

        return single is not null
            ? new[] { single.Permission }
            : null;
    }
}

