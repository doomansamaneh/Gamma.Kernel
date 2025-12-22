using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.Kernel;

public static class DependencyInjection
{
    public static IServiceCollection AddKernel(this IServiceCollection services)
    {
        return services;
    }
}
