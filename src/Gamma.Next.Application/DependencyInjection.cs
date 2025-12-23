using System.Reflection;
using FluentValidation;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Next.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 1. Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        // 2. Register all command handlers
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // 3. Decorate all command handlers with auditing
        services.Decorate(typeof(ICommandHandler<,>), typeof(AuditingCommandHandlerDecorator<,>));

        // 4. Register higher-level application services (business services)
        services.Scan(scan => scan
                .FromAssembliesOf(typeof(DependencyInjection))
                .AddClasses(c => c.AssignableTo<IApplicationService>(), publicOnly: false)
                .AsImplementedInterfaces()
                .As<IApplicationService>()
                .WithScopedLifetime());

        // 5. Decorate all application services with authorization
        //services.Decorate(typeof(IApplicationService), typeof(AuthorizationServiceDecorator<>));
        services.AddServiceDecorator<IApplicationService, AuthorizationServiceDecorator<IApplicationService>>();

        return services;
    }

    public static IServiceCollection AddServiceDecorator<TService, TDecorator>(
        this IServiceCollection services)
        where TService : class
        where TDecorator : DispatchProxy, new()
    {
        // Capture the original registration
        services.Decorate<TService>((inner, provider) =>
        {
            // Create DispatchProxy instance
            var proxy = DispatchProxy.Create<TService, TDecorator>() as TDecorator;

            // Set inner service
            var innerProperty = typeof(TDecorator).GetProperty("Inner")!;
            innerProperty.SetValue(proxy, inner);

            // Inject any other services needed (e.g., IAuthorizationService)
            var authProperty = typeof(TDecorator).GetProperty("Authorization")!;
            authProperty.SetValue(proxy, provider.GetRequiredService<IAuthorizationService>());

            return proxy as TService;
        });

        return services;
    }

}
