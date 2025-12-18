using Gamma.Next.Application.Interfaces;
using Gamma.Next.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Next.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
