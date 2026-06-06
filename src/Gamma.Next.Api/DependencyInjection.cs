using Gamma.Next.Api.Endpoints.Ast;

namespace Gamma.Next.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        return services;
    }

    public static void MapEndPoints(this IEndpointRouteBuilder app)
    {
        //app.MapAuthEndpoints();

        //ast (product information management) schema endpoints
        app.MapProductEndpoints();
        app.MapProductGroupEndpoints();
    }
}
