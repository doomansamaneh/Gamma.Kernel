using FluentValidation;
using Gamma.Kernel.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Next.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.Scan(scan => scan
                .FromAssembliesOf(typeof(DependencyInjection))
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Scan(scan => scan
                .FromAssembliesOf(typeof(DependencyInjection))
                .AddClasses(classes => classes.InNamespaces("Gamma.Next.Application.Services"), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;
    }
}
