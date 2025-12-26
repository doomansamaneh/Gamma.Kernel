using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Persistence;
using Gamma.Kernel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Kernel;

public static class DependencyInjection
{
    public static IServiceCollection AddKernel(this IServiceCollection services)
    {
        //services.AddSingleton<PermissionInterceptor>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ITransactionExecutor, TransactionExecutor>();

        return services;
    }
}
