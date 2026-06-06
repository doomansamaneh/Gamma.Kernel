using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Mediator;
using Gamma.Kernel.Abstractions;

namespace Gamma.Next.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(
           options =>
           {
               options.Assemblies = [typeof(DependencyInjection)];
               options.ServiceLifetime = ServiceLifetime.Scoped;
           }
       );

        //services.AddScoped<IAuthorizationService, Services.AuthorizationService>();

        services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Kernel.Pipelines.ValidationPipeline<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Kernel.Pipelines.AuthorizationPipeline<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Kernel.Pipelines.UnitOfWorkPipeline<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Kernel.Pipelines.AuditPipeline<,>))
            ;

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
                 .AddClasses(c => c.AssignableTo<IApplicationService>()
                                    .Where(t => !t.IsAbstract), publicOnly: false)
                 .AsImplementedInterfaces()
                 .WithScopedLifetime()
                );

        return services;
    }
}
