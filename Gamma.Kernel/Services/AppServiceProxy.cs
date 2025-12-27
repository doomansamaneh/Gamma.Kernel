using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Security;

namespace Gamma.Kernel.Services;

public class AppServiceProxy<T> : DispatchProxy where T : class
{
    private static readonly ConcurrentDictionary<MethodInfo, MethodAuthorizationMetadata> _authorizationCache = new();
    private T? _decorated;
    private IAuthorizationService? _authorizeService;

    private MethodAuthorizationMetadata GetAuthorizationMetadata(MethodInfo targetMethod)
    {
        return _authorizationCache.GetOrAdd(targetMethod, tm =>
        {
            var implMethod = _decorated!.GetType().GetMethod(
                tm.Name,
                [.. tm.GetParameters().Select(p => p.ParameterType)])
            ?? throw new InvalidOperationException($"Implementation method not found for {tm.Name}");
            var attr = implMethod.GetCustomAttribute<RequiresPermissionAttribute>(true);

            return attr is null
                ? new MethodAuthorizationMetadata { RequiresAuthorization = false }
                : new MethodAuthorizationMetadata
                {
                    RequiresAuthorization = true,
                    Permission = attr.Permission
                };
        });
    }

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        if (_decorated is null) throw new InvalidOperationException();
        if (_authorizeService is null) throw new InvalidOperationException();

        var meta = GetAuthorizationMetadata(targetMethod);

        if (meta.RequiresAuthorization)
        {
            var hasPermission = _authorizeService
                .HasPermissionAsync(meta.Permission!)
                .GetAwaiter()
                .GetResult();

            if (!hasPermission)
                throw new UnauthorizedAccessException(
                    $"Permission {meta.Permission} required."
                );
        }
        else
        {
            throw new UnauthorizedAccessException($"Permission attribute not set on method {targetMethod.Name}.");
        }

        var result = targetMethod.Invoke(_decorated, args);

        if (targetMethod.ReturnType == typeof(void))
            return null!;

        return result!;
    }

    public static TService Create<TService>(
        TService decorated,
        IAuthorizationService authorizeService
    ) where TService : class
    {
        ArgumentNullException.ThrowIfNull(decorated);
        ArgumentNullException.ThrowIfNull(authorizeService);

        if (Create<TService, AppServiceProxy<TService>>() is not AppServiceProxy<TService> proxy)
            throw new InvalidOperationException("Failed to create proxy instance.");

        proxy._decorated = decorated;
        proxy._authorizeService = authorizeService;
        return proxy as TService ?? throw new InvalidOperationException("Failed to cast proxy to service type.");
    }
}
