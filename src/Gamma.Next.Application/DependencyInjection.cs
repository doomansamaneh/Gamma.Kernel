using System.Reflection;
using FluentValidation;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Services;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

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

        services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Pipelines.AuthorizationPipeline<,>))
            //.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Pipelines.UnitOfWorkPipeline<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Pipelines.AuditLoggingPipeline<,>))
            //.AddScoped(typeof(IPipelineBehavior<,>), typeof(Pipelines.ErrorLoggingBehaviour<,>))
            ;

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler_<,>))
                                .Where(t => !t.IsAbstract), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
                 .AddClasses(c => c.AssignableTo<IApplicationService>()
                                    .Where(t => !t.IsAbstract), publicOnly: false)
                 .AsImplementedInterfaces()
                 .WithScopedLifetime()
                );

        //services.DecorateProxies();

        return services;
    }

    /// <summary>
    /// decorate all service method calls viw proxy
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static IServiceCollection DecorateProxies(this IServiceCollection services)
    {
        var appServiceTypes = services
            .Where(d => typeof(IApplicationService).IsAssignableFrom(d.ServiceType))
            .Select(d => d.ServiceType)
            .ToList();

        foreach (var type in appServiceTypes)
        {
            services.Decorate(type, (inner, sp) =>
            {
                // Resolve the authorize service from the DI container
                var authorizeService = sp.GetRequiredService<IAuthorizationService>();

                // Make a closed AppServiceProxy<type>
                var proxyType = typeof(AppServiceProxy<>).MakeGenericType(type);

                // Get the static Create<TService> method (generic)
                var createMethod = proxyType.GetMethod(
                    "Create",
                    BindingFlags.Public | BindingFlags.Static
                ) ?? throw new InvalidOperationException("Cannot find Create method");

                // Make the method generic with the same type
                var genericMethod = createMethod.MakeGenericMethod(type);

                // Invoke Create with inner + authorizeService
                var proxy = genericMethod.Invoke(null, [inner, authorizeService])
                    ?? throw new InvalidOperationException("Proxy creation returned null");

                return proxy;
            });
        }
        return services;
    }
}
