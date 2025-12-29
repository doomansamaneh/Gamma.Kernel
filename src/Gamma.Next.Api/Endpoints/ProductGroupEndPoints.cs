using Gamma.Next.Application.Commands.Product;
using Gamma.Next.Application.Commands.ProductGroup;
using Gamma.Next.Application.Interfaces;

namespace Gamma.Next.Api.Endpoints;

public static class ProductGroupEndpoints
{
    public static void MapProductGroupEndPoints(this IEndpointRouteBuilder app)
    {
        // Create / Add
        app.MapPost("/product-groups", async (
            CreateProductGroupCommand command,
            IProductGroupCommandService service,
            CancellationToken ct
        ) =>
        {
            var result = await service.CreateAsync(command, ct);
            return result.Success
                ? Results.Created($"/product-groups/{result.Data}", result.Data)
                : Results.BadRequest(result.Errors);
        });

        // // Edit
        // app.MapPut("/product-groups/{id:guid}", async (
        //     Guid id,
        //     UpdateProductGroupCommand command,
        //     IProductGroupCommandService service,
        //     CancellationToken ct
        // ) =>
        // {
        //     command.Id = id;
        //     var result = await service.UpdateAsync(command, ct);
        //     return result.Success
        //         ? Results.Ok(result.Data)
        //         : Results.BadRequest(result.Errors);
        // });

        // Delete
        // app.MapDelete("/product-groups/{id:guid}", async (
        //     Guid id,
        //     IProductGroupCommandService service,
        //     CancellationToken ct
        // ) =>
        // {
        //     var command = new DeleteProductGroupCommand { Id = id };
        //     var result = await service.DeleteAsync(command, ct);
        //     return result.Success
        //         ? Results.Ok(result.Data)
        //         : Results.BadRequest(result.Errors);
        // });

        // test add service
        app.MapGet("/test-product-groups", async (
                IProductGroupCommandService service,
                CancellationToken ct) =>
        {
            var command = new CreateProductGroupCommand(new ProductGroupInput
            {
                Code = "t001",
                Title = "Test Product Group",
                Comment = "This is a test product group",
                IsActive = true
            });
            command.ProductGroup.Products.Add(
                new ProductInput
                {
                    Code = "p001",
                    Title = "Test Product 1",
                    Comment = "This is test product 1",
                    IsActive = true
                });
            command.ProductGroup.Products.Add(
                new ProductInput
                {
                    Code = "p002",
                    Title = "Test Product 2",
                    Comment = "This is test product 2",
                    IsActive = true
                });

            var result = await service.CreateAsync(command, ct);
            return result.Success
                    ? Results.Ok(new { Id = result.Data, command.ProductGroup.Code, command.ProductGroup.Title })
                    : Results.BadRequest(result.Errors);
        });
    }
}
