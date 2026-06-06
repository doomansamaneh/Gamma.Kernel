using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Infra;
using Gamma.Kernel.Security;
using Gamma.Kernel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Kernel;

public static class DependencyInjection
{
    public static IServiceCollection AddKernel(this IServiceCollection services)
    {
        //services.AddSingleton<IUidGenerator, UidGenerator>();
        services.AddSingleton<IUidGenerator, SqlServerUidGenerator>();
        services.AddSingleton<ISystemClock, SystemClockService>();

        services.AddScoped<IAuthorizationContext, AuthorizationContext>();
        services.AddScoped<IAuditMetadataResolver, Logging.DefaultAuditMetadataResolver>();

        return services;
    }
}
