#nullable enable
using Mediator;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Commands;
using Gamma.Next.Application.Ast.Product.Commands;
using Gamma.Next.Application.Ast.Product.Dtos;
using Gamma.Next.Application.Ast.Product.Queries;
using Gamma.Next.Api.Extensions;

namespace Gamma.Next.Api.Endpoints.Ast;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/ast/product")
                          .WithTags("Ast.Product")
                          .RequireAuthorization();

        apiGroup.MapPost("/create", async (
                CreateProductCommand command,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.ToCreatedResponse($"/ast/product/{result.Data}");
        });

        apiGroup.MapPost("/edit", async (
                UpdateProductCommand command,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/activate", async (
                BatchActionCommandBase command,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var activateCmd = new ActivateProductCommand(command.Ids, true);
            var result = await mediator.Send(activateCmd, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/deactivate", async (
                BatchActionCommandBase command,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var activateCmd = new ActivateProductCommand(command.Ids, false);
            var result = await mediator.Send(activateCmd, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("delete/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await mediator.Send(command, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/delete-batch", async (
                DeleteBatchProductCommand command,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapGet("/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new ProductByIdQuery(id);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/data-grid", async (
                PageModel<ProductSearchDto> page,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new ProductGridQuery(page);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/lookup", async (
                PageModel<ProductSearchDto> page,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new ProductLookupQuery(page);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });

         apiGroup.MapPost("/export", async (
                PageFilterModel<ProductSearchDto> filter,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new ProductExportQuery(filter);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });
    }
}
