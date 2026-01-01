using System.Collections.Concurrent;
using System.Reflection;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Security;

namespace Gamma.Kernel.Services;

public class AppServiceProxy<T> : DispatchProxy
    where T : class
{
    private static readonly ConcurrentDictionary<MethodInfo, MethodAuthorizationMetadata> AuthorizationCache = new();

    private T? _decorated;
    private IAuthorizationService? _authorizationService;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(targetMethod);

        if (_decorated is null) throw new InvalidOperationException("Proxy is not initialized.");

        if (_authorizationService is null) throw new InvalidOperationException("Authorization service is not initialized.");

        var metadata = GetAuthorizationMetadata(targetMethod);

        if (metadata.RequiresAuthorization)
        {
            var allowed = false;

            foreach (var permission in metadata.Permissions)
            {
                if (_authorizationService
                        .HasPermissionAsync(permission)
                        .GetAwaiter()
                        .GetResult())
                {
                    allowed = true;
                    break;
                }
            }

            if (!allowed)
            {
                throw new UnauthorizedAccessException(
                    $"Requires any of permissions: {string.Join(", ", metadata.Permissions)}");
            }
        }

        var result = targetMethod.Invoke(_decorated, args);

        return targetMethod.ReturnType == typeof(void)
            ? null
            : result;
    }


    private MethodAuthorizationMetadata GetAuthorizationMetadata(MethodInfo interfaceMethod)
    {
        return AuthorizationCache.GetOrAdd(interfaceMethod, static (im, state) =>
        {
            var proxy = (AppServiceProxy<T>)state;

            var implementationMethod = proxy._decorated!.GetType().GetMethod(
                im.Name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                types: [.. im.GetParameters().Select(p => p.ParameterType)],
                modifiers: null
            ) ??
             throw new InvalidOperationException($"Implementation method not found for {im.DeclaringType?.Name}.{im.Name}");

            var permissions = implementationMethod
                .GetCustomAttributes<RequiresPermissionAttribute>(true)
                .Select(a => a.Permission)
                .Distinct()
                .ToArray();

            return permissions.Length == 0
                ? new MethodAuthorizationMetadata
                {
                    RequiresAuthorization = false
                }
                : new MethodAuthorizationMetadata
                {
                    RequiresAuthorization = true,
                    Permissions = permissions
                };
        }, this);
    }

    public static TService Create<TService>(
        TService decorated,
        IAuthorizationService authorizationService)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(decorated);
        ArgumentNullException.ThrowIfNull(authorizationService);

        var proxy = Create<TService, AppServiceProxy<TService>>()
            as AppServiceProxy<TService>
            ?? throw new InvalidOperationException("Failed to create proxy.");

        proxy._decorated = decorated;
        proxy._authorizationService = authorizationService;

        return (proxy as TService)!;
    }
}