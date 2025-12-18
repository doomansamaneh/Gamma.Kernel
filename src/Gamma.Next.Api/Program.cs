using Gamma.Next.Api.Endpoints;
using Gamma.Next.Application;
using Gamma.Next.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddApplication()
    .AddInfra(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () =>
    Results.Ok(new
    {
        name = "Gamma.Next API",
        status = "Running",
        time = DateTime.Now
    }));

app.MapCustomerEndpoints();

app.Run();
