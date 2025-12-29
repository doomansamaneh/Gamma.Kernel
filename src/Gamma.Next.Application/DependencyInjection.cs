using System.Reflection;
using FluentValidation;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Behaviors;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Next.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>))
                                .Where(t => !t.IsAbstract), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
                 .AddClasses(c => c.AssignableTo<IApplicationService>()
                                    .Where(t => !t.IsAbstract), publicOnly: false)
                 .AsImplementedInterfaces()
                 .WithScopedLifetime()
                );

        //services.Decorate(typeof(ICommandHandler<,>), typeof(AuditingCommandHandlerDecorator<,>));
        //services.Decorate(typeof(ICommandService<,,,>), typeof(CommandServiceAuthorizationDecorator<,,,>));
        services.DecorateProxies();

        return services;
    }

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
