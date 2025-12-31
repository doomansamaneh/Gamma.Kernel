using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Paging;
using Gamma.Next.Application.Commands.Product;
using Gamma.Next.Application.ProductGroup;
using Gamma.Next.Application.ProductGroup.Commands;
using Gamma.Next.Application.ProductGroup.Dtos;
using Gamma.Next.Application.ProductGroup.Queries;

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
        app.MapGet("/test-add-pg", async (
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

        // test add service
        app.MapGet("/test-update-pg", async (
                IProductGroupCommandService service,
                CancellationToken ct) =>
        {
            var command = new UpdateProductGroupCommand(new ProductGroupInput
            {
                Code = "t001",
                Title = "Test Product Group",
                Comment = "This is a test product group",
                IsActive = true,
            })
            {
                Id = new("019B74F7-84D6-7741-B1A3-433144D33679"),
                RowVersion = 1
            };

            var result = await service.UpdateAsync(command, ct);
            return result.Success
                    ? Results.Ok(new { Id = result.Data, command.ProductGroup.Code, command.ProductGroup.Title })
                    : Results.BadRequest(result.Errors);
        });

        app.MapGet("/test-get-pg", async (
                IQueryHandler<GetProductGroupsQuery, PagedResult<ProductGroupDto>> handler,
                CancellationToken ct) =>
        {
            var pageModel = new PageModel<ProductGroupSearch>
            {
                Page = 1,
                PageSize = 10,
                SearchTerm = null,
                SortBy = "Id",
                SortOrder = SortOrder.Descending,
                Search = new ProductGroupSearch("PG00", null, null)
            };

            var query = new GetProductGroupsQuery(pageModel);

            var result = await handler.HandleAsync(query, ct);

            return Results.Ok(result);
        });
    }
}
