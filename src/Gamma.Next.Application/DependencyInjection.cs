using System.Reflection;
using FluentValidation;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Behaviors;
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
        //  .AddClasses(c => c.AssignableTo(typeof(ICommandService<,,,>)), publicOnly: false)
        //  .AsImplementedInterfaces()
        //  .WithScopedLifetime()
                );

        services.Decorate(typeof(ICommandHandler<,>), typeof(AuditingCommandHandlerDecorator<,>));
        //services.Decorate(typeof(ICommandService<,,,>), typeof(CommandServiceAuthorizationDecorator<,,,>));
        services.DecorateProxies();
        //services.AddGeneratedApplicationServices();

        return services;
    }

    private static IServiceCollection AddGeneratedApplicationServices(this IServiceCollection services)
    {
        // پیدا کردن assembly که IApplicationService داخل آن است
        var assembly = typeof(DependencyInjection).Assembly;

        // همه interfaceهای IApplicationService
        var interfaces = assembly.GetTypes()
            .Where(t => t.IsInterface && typeof(IApplicationService).IsAssignableFrom(t))
            .ToList();

        foreach (var iface in interfaces)
        {
            // پیاده‌سازی واقعی (concrete class)
            var impl = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && iface.IsAssignableFrom(t));

            if (impl == null) continue;

            // نام decorator generated
            var decoratorName = $"{iface.Name}Decorator";

            // پیدا کردن decorator type
            var decoratorFullName = $"Gamma.Next.ApplicationServiceGenerator.{iface.Name}Decorator";
            //var decoratorFullName = $"Gamma.Next.ApplicationServiceGenerator.{iface.Name}Decorator";
            var decoratorType = assembly.GetTypes()
                .FirstOrDefault(t => t.FullName == decoratorFullName);

            if (decoratorType == null) continue;

            // register اصلی
            services.AddScoped(iface, sp =>
            {
                var inner = ActivatorUtilities.CreateInstance(sp, impl);
                return ActivatorUtilities.CreateInstance(sp, decoratorType, inner);
            });
        }

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
