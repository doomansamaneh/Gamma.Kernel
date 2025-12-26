using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Security;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Behaviors;

public class AuditingCommandServiceDecorator(ITransactionExecutor innerService
    , IAuthorizationService authorizationService) : ITransactionExecutor
{
    public async Task<Result<T>> ExecuteAsync<T>(Func<IUnitOfWork, Task<Result<T>>> action
        , CancellationToken ct = default)
    {
        // Get the method that is being executed
        var method = action.Method;

        // Extract any permission attributes applied to the method
        var permissionAttributes = method.GetCustomAttributes(typeof(RequiresPermissionAttribute), false)
                                          .Cast<RequiresPermissionAttribute>()
                                          .ToList();

        if (permissionAttributes.Count != 0)
        {
            // Check if the user has the necessary permissions
            foreach (var permission in permissionAttributes)
            {
                var hasPermission = await authorizationService.HasPermissionAsync(permission.Permission, ct);

                if (!hasPermission)
                {
                    // If the user doesn't have permission, return Forbidden
                    return Result<T>.Fail("Forbidden: You do not have permission to execute this action.");
                }
            }
        }

        // If permissions are valid or no permissions are needed, proceed with the action
        var result = await innerService.ExecuteAsync(action, ct);

        // Optionally log the result after execution
        Console.WriteLine($"Action result: {(result.Success ? "Success" : "Failure")}");

        return result;
    }
}

