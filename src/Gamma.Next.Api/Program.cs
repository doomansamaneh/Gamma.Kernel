using Gamma.Kernel.Web.Extensions;
using Gamma.Kernel;
using Gamma.Next.Api.Endpoints;
using Gamma.Next.Application;
using Gamma.Next.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddKernel()
    .AddApplication()
    .AddInfra();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

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
