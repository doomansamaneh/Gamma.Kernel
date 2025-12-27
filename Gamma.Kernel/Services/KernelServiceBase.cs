using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Exceptions;

namespace Gamma.Kernel.Services;

public abstract class KernelServiceBase(IAuthorizationService authorizationService) : IApplicationService
{
    public virtual void DoTest()
    {
        Console.WriteLine("kernel service log");
    }

    protected async Task EnsurePermissionAsync(string permission, CancellationToken ct)
    {
        var allowed = await authorizationService.HasPermissionAsync(permission, ct);
        if (!allowed) throw new ForbiddenException(permission);
    }
}
