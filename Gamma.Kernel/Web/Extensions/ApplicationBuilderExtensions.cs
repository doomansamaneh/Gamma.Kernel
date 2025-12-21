using Gamma.Kernel.Web.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Gamma.Kernel.Web.Extensions;

public static class KernelMiddlewareExtensions
{
    public static IApplicationBuilder UseKernelMiddleware<TMiddleware>(
        this IApplicationBuilder app)
        where TMiddleware : class
    {
        return app.UseMiddleware<TMiddleware>();
    }

    public static IApplicationBuilder UseKernelDefaults(this IApplicationBuilder app)
    {
        return app
            .UseKernelMiddleware<GlobalExceptionMiddleware>()
            //.UseKernelMiddleware<RequestLoggingMiddleware>()
            //.UseKernelMiddleware<CorrelationIdMiddleware>()
            // ;
    }
}
