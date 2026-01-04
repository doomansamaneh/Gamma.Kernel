using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Data;
using Gamma.Kernel.Infra;
using Gamma.Kernel.Security;
using Gamma.Kernel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Kernel;

public static class DependencyInjection
{
    public static IServiceCollection AddKernel(this IServiceCollection services)
    {
        services.AddSingleton<IUidGenerator, UidGenerator>();
        services.AddSingleton<ISystemClock, SystemClockService>();

        services.AddScoped<IAuthorizationContext, AuthorizationContext>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        // Semantic APIs
        // services.AddScoped(typeof(ICreateCommandHandler<>), typeof(GenericCreateCommandHandler<>));
        // services.AddScoped(typeof(IUpdateCommandHandler<>), typeof(GenericUpdateCommandHandler<>));
        // services.AddScoped(typeof(IDeleteCommandHandler<,>), typeof(GenericDeleteCommandHandler<,>));

        return services;
    }
}
