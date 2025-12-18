using Gamma.Next.Application.Commands.Customer;
using Gamma.Next.Application.Commands.CustomerAddress;
using Gamma.Next.Application.Commands.Shared;
using Gamma.Next.Application.Interfaces;
using Gamma.Next.Application.Queries.Customer;
using Microsoft.AspNetCore.Routing;

namespace Gamma.Next.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        // ----------------------------
        // Customer Queries
        // ----------------------------
        app.MapGet("/customers", async (ICustomerQueryService service, string? name, string? city, int page = 1, int pageSize = 20) =>
        {
            var query = new CustomerQuery
            {
                Name = name,
                City = city,
                Page = page,
                PageSize = pageSize
            };
            var result = await service.GetAsync(query);
            return Results.Ok(result);
        });

        // ----------------------------
        // Customer Commands
        // ----------------------------
        app.MapPost("/customers", async (ICustomerCommandService service, AddCustomerCommand command) =>
        {
            var id = await service.AddCustomerAsync(command);
            return Results.Created($"/customers/{id}", id);
        });

        app.MapPut("/customers/{id:guid}", async (ICustomerCommandService service, Guid id, EditCustomerCommand command) =>
        {
            command.Id = id;
            var success = await service.EditCustomerAsync(command);
            return success ? Results.Ok() : Results.NotFound();
        });

        app.MapDelete("/customers/{id:guid}", async (ICustomerCommandService service, Guid id) =>
        {
            var success = await service.DeleteCustomerAsync(new DeleteCommand { Id = id });
            return success ? Results.Ok() : Results.NotFound();
        });

        // ----------------------------
        // CustomerAddress Commands
        // ----------------------------
        app.MapPost("/customers/{customerId:guid}/addresses", async (ICustomerCommandService service, Guid customerId, CustomerAddressInput input) =>
        {
            var command = new AddCustomerAddressCommand
            {
                CustomerId = customerId,
                Address = input
            };
            var id = await service.AddAddressAsync(command);
            return Results.Created($"/customers/addresses/{id}", id);
        });

        app.MapPut("/customers/addresses/{id:guid}", async (ICustomerCommandService service, Guid id, CustomerAddressInput input) =>
        {
            var command = new EditCustomerAddressCommand
            {
                Id = id,
                Address = input
            };
            var success = await service.EditAddressAsync(command);
            return success ? Results.Ok() : Results.NotFound();
        });

        app.MapDelete("/customers/addresses/{id:guid}", async (ICustomerCommandService service, Guid id) =>
        {
            var success = await service.DeleteAddressAsync(new DeleteCommand { Id = id });
            return success ? Results.Ok() : Results.NotFound();
        });
    }
}
