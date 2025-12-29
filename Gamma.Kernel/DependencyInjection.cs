using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Infra;
using Gamma.Kernel.Persistence;
using Gamma.Kernel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Kernel;

public static class DependencyInjection
{
    public static IServiceCollection AddKernel(this IServiceCollection services)
    {
        services.AddSingleton<IUidGenerator, UidGenerator>();
        services.AddSingleton<ISystemClock, SystemClockService>();

        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ITransactionExecutor, TransactionExecutor>();

        // Register open-generic CRUD handlers
        services.AddScoped(typeof(ICreateCommandHandler<>), typeof(CreateCommandHandler<>));
        services.AddScoped(typeof(IUpdateCommandHandler<>), typeof(UpdateCommandHandler<>));
        services.AddScoped(typeof(IDeleteCommandHandler<,>), typeof(DeleteCommandHandler<,>));

        return services;
    }
}
