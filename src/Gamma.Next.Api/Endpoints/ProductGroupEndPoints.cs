using Gamma.Next.Application.Commands.ProductGroup;
using Gamma.Next.Application.Commands.Shared;
using Gamma.Next.Application.Interfaces;

namespace Gamma.Next.Api.Endpoints;

public static class ProductGroupEndpoints
{
    public static void MapProductGroupEndPoints(this IEndpointRouteBuilder app)
    {
        // Create / Add
        app.MapPost("/product-groups", async (
            AddProductGroupCommand command,
            IProductGroupService service,
            CancellationToken ct
        ) =>
        {
            var result = await service.AddAsync(command, ct);
            return result.Success
                ? Results.Created($"/product-groups/{result.Data}", result.Data)
                : Results.BadRequest(result.Errors);
        });

        // Edit
        app.MapPut("/product-groups/{id:guid}", async (
            Guid id,
            EditProductGroupCommand command,
            IProductGroupService service,
            CancellationToken ct
        ) =>
        {
            command.Id = id;
            var result = await service.EditAsync(command, ct);
            return result.Success
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors);
        });

        // Delete
        app.MapDelete("/product-groups/{id:guid}", async (
            Guid id,
            IProductGroupService service,
            CancellationToken ct
        ) =>
        {
            var command = new DeleteProductGroupCommand { Id = id };
            var result = await service.DeleteAsync(command, ct);
            return result.Success
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors);
        });

        // test add service
        app.MapGet("/test-product-groups", async (IProductGroupService service, CancellationToken ct) =>
        {
            var command = new AddProductGroupCommand(new ProductGroupInput
            {
                Code = "t001",
                Title = "Test Product Group",
                Comment = "This is a test product group",
                IsActive = true
            });

            var result = await service.AddAsync(command, ct);
            return result.Success
                    ? Results.Ok(new { Id = result.Data, command.ProductGroup.Code, command.ProductGroup.Title })
                    : Results.BadRequest(result.Errors);
        });
    }
}
