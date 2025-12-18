using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Services;
using Gamma.Next.Application.Interfaces;
using Gamma.Next.Domain.Interfaces;
using Gamma.Next.Infra.Data;
using Gamma.Next.Infra.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Next.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSingleton(
            new DapperContext(config.GetConnectionString("Default")!));

        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<ISystemClock, SystemClockService>();

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
        services.AddScoped<CustomerCommandService, CustomerCommandService>();
        services.AddScoped<ICustomerQueryService, CustomerQueryService>();

        return services;
    }
}
