using Gamma.Kernel.Web.Extensions;
using Gamma.Kernel;
using Gamma.Next.Api.Endpoints;
using Gamma.Next.Application;
using Gamma.Next.Infra;
using Gamma.Next.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddKernel()
    .AddApplication()
    .AddInfra()
    .AddApi();

builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseKernelDefaults();

app.MapGet("/", () =>
    Results.Ok(new
    {
        name = "Gamma.Next API",
        status = "Running",
        time = DateTime.Now
    }));

app.MapProductGroupEndPoints();

app.Run();
