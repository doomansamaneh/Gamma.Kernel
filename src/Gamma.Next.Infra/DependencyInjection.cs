using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Gamma.Kernel.Abstractions;
using Gamma.Next.Infra.Logging;
using Gamma.Next.Infra.Data;

namespace Gamma.Next.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(
        this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<ICurrentUser, Identity.HttpCurrentUser>();
        //TODO: add token service
        //services.AddScoped<ITokenService, JwtTokenService>();

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
                    .AddClasses(c => c.Where(t =>
                            t.GetInterfaces().Any(i => i.IsGenericType
                                                && i.GetGenericTypeDefinition() == typeof(IRepository<>))
                            && !t.IsAbstract))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                    );

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            //.WriteTo.Console()
            .WriteTo.File("logs/audit.json", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Register IAuditLogger singleton
        services.AddSingleton<IAuditLogger>(sp => new SerilogAuditLogger(Log.Logger));

        //TODO: optional db logger
        //services.AddScoped<IAuditLogger, SqlAuditLogger>();

        return services;
    }
}
