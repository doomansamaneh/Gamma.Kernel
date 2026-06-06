#nullable enable
using Mediator;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Commands;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Commands;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Dtos;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Queries;
using {{ApiNamespace}}.Extensions;

namespace {{ApiNamespace}}.Endpoints.{{Schema}};

public static class {{Entity}}Endpoints
{
    public static void Map{{Entity}}Endpoints(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/{{schema_lower}}/{{entity_lower}}")
                          .WithTags("{{Schema}}.{{Entity}}")
                          .RequireAuthorization();

        apiGroup.MapPost("/create", async (
                Create{{Entity}}Command command,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.ToCreatedResponse($"/{{schema_lower}}/{{entity_lower}}/{result.Data}");
        });

        apiGroup.MapPost("/edit", async (
                Update{{Entity}}Command command,
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
            var activateCmd = new Activate{{Entity}}Command(command.Ids, true);
            var result = await mediator.Send(activateCmd, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/deactivate", async (
                BatchActionCommandBase command,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var activateCmd = new Activate{{Entity}}Command(command.Ids, false);
            var result = await mediator.Send(activateCmd, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("delete/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var command = new Delete{{Entity}}Command { Id = id };
            var result = await mediator.Send(command, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/delete-batch", async (
                DeleteBatch{{Entity}}Command command,
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
            var query = new {{Entity}}ByIdQuery(id);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/data-grid", async (
                PageModel<{{Entity}}SearchDto> page,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new {{Entity}}GridQuery(page);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });

        apiGroup.MapPost("/lookup", async (
                PageModel<{{Entity}}SearchDto> page,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new {{Entity}}LookupQuery(page);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });

         apiGroup.MapPost("/export", async (
                PageFilterModel<{{Entity}}SearchDto> filter,
                IMediator mediator,
                CancellationToken ct) =>
        {
            var query = new {{Entity}}ExportQuery(filter);
            var result = await mediator.Send(query, ct);
            return result.ToApiResponse();
        });
    }
}
