using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Data;
using Gamma.Kernel.Services;
using Gamma.Next.Infra.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Next.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(
        this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddSingleton<IUnitOfWorkFactory, DapperUnitOfWorkFactory>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICurrentUser, Identity.HttpCurrentUser>();
        services.AddScoped<ISystemClock, SystemClockService>();

        return services;
    }
}
