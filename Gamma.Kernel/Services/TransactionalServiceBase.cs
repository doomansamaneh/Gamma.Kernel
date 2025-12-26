using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Exceptions;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Services;

public abstract class KernelServiceBase(IAuthorizationService authorizationService) : IApplicationService
{
    protected async Task EnsurePermissionAsync(string permission, CancellationToken ct)
    {
        var allowed = await authorizationService.HasPermissionAsync(permission, ct);
        if (!allowed) throw new ForbiddenException(permission);
    }
}

public abstract class TransactionalServiceBase(
        IAuthorizationService authorizationService,
        ITransactionExecutor transactionExecutor
    ) : KernelServiceBase(authorizationService)
{

    protected async Task<Result<T>> ExecuteHandlerAsync<T>(
        Func<IUnitOfWork, Task<Result<T>>> action,
        CancellationToken ct = default)
    {
        return await transactionExecutor.ExecuteAsync(action, ct);
    }
}