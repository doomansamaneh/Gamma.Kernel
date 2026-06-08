#nullable enable
using Mediator;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Commands;
using Gamma.Next.Application.Ast.ProductGroup.Commands;
using Gamma.Next.Application.Ast.ProductGroup.Dtos;
using Gamma.Next.Application.Ast.ProductGroup.Queries;
using Gamma.Next.Api.Extensions;

namespace Gamma.Next.Api.Endpoints.Ast;

public static class ProductGroupEndpoints
{
    public static void MapProductGroupEndpoints(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/ast/productgroup")
                          .WithTags("Ast.ProductGroup")
                          //.RequireAuthorization()
                            ;

        apiGroup.MapPost("/create", async (
                CreateProductGroupCommand command,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.ToCreatedResponse($"/ast/productgroup/{result.Data}");
        });

        apiGroup.MapPost("/edit", async (
                UpdateProductGroupCommand command,
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
            var activateCmd = new ActivateProductGroupCommand(command.Ids, true);
            var result = await mediator.Send(activateCmd, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/deactivate", async (
                BatchActionCommandBase command,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var activateCmd = new ActivateProductGroupCommand(command.Ids, false);
            var result = await mediator.Send(activateCmd, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("delete/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var command = new DeleteProductGroupCommand { Id = id };
            var result = await mediator.Send(command, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/delete-batch", async (
                DeleteBatchProductGroupCommand command,
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
            var query = new ProductGroupByIdQuery(id);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/data-grid", async (
                PageModel<ProductGroupSearchDto> page,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new ProductGroupGridQuery(page);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/lookup", async (
                PageModel<ProductGroupSearchDto> page,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new ProductGroupLookupQuery(page);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });

         apiGroup.MapPost("/export", async (
                PageFilterModel<ProductGroupSearchDto> filter,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new ProductGroupExportQuery(filter);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });
    }
}
