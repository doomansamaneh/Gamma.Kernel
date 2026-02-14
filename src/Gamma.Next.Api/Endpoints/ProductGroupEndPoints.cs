using Gamma.Kernel.Paging;
using Gamma.Next.Application.Product.Commands;
using Gamma.Next.Application.ProductGroup.Commands;
using Gamma.Next.Application.ProductGroup.Dtos;
using Gamma.Next.Application.ProductGroup.Queries;
using Mediator;

namespace Gamma.Next.Api.Endpoints;

public static class ProductGroupEndpoints
{
    public static void MapProductGroupEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/product-group", async (
            CreateProductGroupCommand command,
            IMediator mediator,
            CancellationToken ct
        ) =>
        {
            var result = await mediator.Send(command, ct);
            return result.Success
                ? Results.Created($"/product-group/{result.Data}", result.Data)
                : Results.BadRequest(result.Errors);
        });

        app.MapPut("/product-group/{id:guid}", async (
            Guid id,
            UpdateProductGroupCommand command,
            IMediator mediator,
            CancellationToken ct
        ) =>
        {
            command.Id = id;
            var result = await mediator.Send(command, ct);
            return result.Success
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors);
        });

        app.MapDelete("/product-group/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct
        ) =>
        {
            var command = new DeleteProductGroupCommand { Id = id };
            var result = await mediator.Send(command, ct);
            return result.Success
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors);
        });

        app.MapGet("/product-group/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct
        ) =>
        {
            var command = new GetProductGroupByIdQuery(id);
            var result = await mediator.Send(command, ct);
            return result.Success
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors);
        });

        // app.MapGet("/prdouct-group/data-grid", async (
        //         IMediator mediator,
        //         CancellationToken ct) =>
        // {
        //     var pageModel = new PageModel<ProductGroupSearch>
        //     {
        //         Page = 1,
        //         PageSize = 10,
        //         SearchTerm = null,
        //         SortBy = "Id",
        //         SortOrder = SortOrder.Descending,
        //         Search = new ProductGroupSearch("PG00", null, null)
        //     };

        //     var query = new GetProductGroupQuery(pageModel);

        //     var result = await mediator.Send(query, ct);

        //     return Results.Ok(result);
        // });

        app.MapPost("/product-group/get-data-grid", async (
                PageModel<ProductGroupSearch> page,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new GetProductGroupQuery(page);
            var result = await mediator.Send(query, ct);

            return Results.Ok(result);
        });


        app.MapPost("/product", async (
           CreateProductCommand command,
           IMediator mediator,
           CancellationToken ct
       ) =>
       {
           var result = await mediator.Send(command, ct);
           return result.Success
               ? Results.Created($"/product/{result.Data}", result.Data)
               : Results.BadRequest(result.Errors);
       });

        app.MapDelete("/product/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct
        ) =>
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await mediator.Send(command, ct);
            return result.Success
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors);
        });
    }
}
