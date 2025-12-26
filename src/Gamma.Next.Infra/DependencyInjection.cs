using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Data;
using Gamma.Kernel.Persistence;
using Gamma.Kernel.Services;
using Gamma.Next.Infra.Data;
using Gamma.Next.Infra.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            //.WriteTo.Console()
            .WriteTo.File("logs/audit.json", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Register IAuditLogger singleton
        services.AddSingleton<IAuditLogger>(sp => new SerilogAuditLogger(Log.Logger));

        //optional db logger
        //services.AddScoped<IAuditLogger, SqlAuditLogger>();

        return services;
    }
}
