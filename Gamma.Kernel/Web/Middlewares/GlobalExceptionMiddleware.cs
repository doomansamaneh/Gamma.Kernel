using Gamma.Kernel.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Gamma.Kernel.Web.Middlewares;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            HandleException(context, ex);
        }
    }

    private void HandleException(HttpContext context, Exception exception)
    {
        // Log only unexpected exceptions
        if (exception is not IExpectedException)
        {
            logger.LogError(exception, "Unhandled exception occurred");
        }

        var error = ExceptionMapper.Map(exception);

        context.Response.StatusCode = MapHttpStatusCode(error.Code);
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(error);
        context.Response.WriteAsync(json);
    }

    private static int MapHttpStatusCode(int errorCode)
    {
        return errorCode switch
        {
            (int)Enums.ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
            (int)Enums.ErrorCodes.ValidationError => StatusCodes.Status400BadRequest,
            (int)Enums.ErrorCodes.BusinessRuleViolation => StatusCodes.Status400BadRequest,
            (int)Enums.ErrorCodes.DatabaseUniqueKey => StatusCodes.Status409Conflict,
            (int)Enums.ErrorCodes.DatabaseForeignKey => StatusCodes.Status409Conflict,
            (int)Enums.ErrorCodes.ConcurrencyConflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
