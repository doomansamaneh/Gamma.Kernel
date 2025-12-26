using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Behaviors;

public class AuditingServiceDecorator(IExecuteHandlerService innerService) : IExecuteHandlerService
{
    public async Task<Result<T>> ExecuteHandlerAsync<T>(Func<IUnitOfWork, Task<Result<T>>> action
        , CancellationToken ct = default)
    {
        // Log the request before execution
        Console.WriteLine($"Executing action of type {action.Method.Name}");

        var result = await innerService.ExecuteHandlerAsync(action, ct);

        // Log the result after execution
        Console.WriteLine($"Action result: {(result.Success ? "Success" : "Failure")}");

        return result;
    }
}

