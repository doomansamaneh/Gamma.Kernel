using System.Reflection;
using Castle.DynamicProxy;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Security;

namespace Gamma.Kernel.Services;

public class PermissionInterceptor(IAuthorizationService authorizationService) : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var method = invocation.MethodInvocationTarget
                     ?? invocation.Method;

        var attribute = method.GetCustomAttribute<RequiresPermissionAttribute>();

        if (attribute != null)
        {
            authorizationService.HasPermissionAsync(attribute.Permission).GetAwaiter().GetResult();
        }

        invocation.Proceed();
    }
}

