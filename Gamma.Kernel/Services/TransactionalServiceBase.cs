using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Services;

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