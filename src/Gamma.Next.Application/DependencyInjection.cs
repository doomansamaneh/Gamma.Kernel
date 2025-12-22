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
            .AddClasses(classes => classes.InNamespaces("Gamma.Next.Application.Services"), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

}
