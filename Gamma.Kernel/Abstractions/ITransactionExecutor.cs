using Gamma.Kernel.Models;

namespace Gamma.Kernel.Abstractions;

public interface ITransactionExecutor
{
    Task<Result<T>> ExecuteAsync<T>(Func<IUnitOfWork, Task<Result<T>>> action, CancellationToken ct = default);
}