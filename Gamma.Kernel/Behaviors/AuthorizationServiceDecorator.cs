using System.Reflection;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Exceptions;
using Gamma.Kernel.Security;

namespace Gamma.Kernel.Behaviors;

public class AuthorizationServiceDecorator<TService> : DispatchProxy
    where TService : class, IApplicationService
{
    public TService Inner { get; set; } = default!;
    public IAuthorizationService Authorization { get; set; } = default!;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod is null) throw new ArgumentNullException(nameof(targetMethod));

        // Check permission attributes dynamically
        var attrs = targetMethod.GetCustomAttributes<RequiresPermissionAttribute>(true)
                    .Concat(targetMethod.DeclaringType!.GetCustomAttributes<RequiresPermissionAttribute>(true));

        foreach (var attr in attrs)
        {
            var allowed = Authorization.HasPermissionAsync(attr.Permission, attr.Resource)
                .GetAwaiter().GetResult();
            if (!allowed) throw new ForbiddenException(attr.Permission);
        }

        // Automatically call the original method
        var result = targetMethod.Invoke(Inner, args);

        // Handle async return
        if (result is Task task)
        {
            task.GetAwaiter().GetResult();
            if (task.GetType().IsGenericType) return ((dynamic)task).Result;
            return null;
        }

        return result;
    }
}

public class AuthorizationServiceDecorator_<TService>(TService inner, IAuthorizationService authorization)
    : IApplicationService
    where TService : class, IApplicationService
{

    public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action, MethodInfo methodInfo)
    {
        // Check RequiresPermission attributes
        var attrs = methodInfo.GetCustomAttributes<RequiresPermissionAttribute>(true)
                    .Concat(methodInfo.DeclaringType!.GetCustomAttributes<RequiresPermissionAttribute>(true));

        foreach (var attr in attrs)
        {
            var allowed = await authorization.HasPermissionAsync(attr.Permission, attr.Resource);
            if (!allowed) throw new ForbiddenException(attr.Permission);
        }

        return await action();
    }
}
