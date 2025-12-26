using Gamma.Kernel.Models;

namespace Gamma.Kernel.Abstractions;

public interface IApplicationService
{
}

public interface IExecuteHandlerService
{
    Task<Result<T>> ExecuteHandlerAsync<T>(
        Func<IUnitOfWork, Task<Result<T>>> action,
        CancellationToken ct = default);
}