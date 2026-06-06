using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Gamma.Kernel.Exceptions;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Web.Middlewares;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log only unexpected exceptions
        if (exception is not IExpectedException)
        {
            logger.LogError(exception, "Unhandled exception occurred");
        }

        var error = ExceptionMapper.Map(exception);

        var statusCode = MapHttpStatusCode(error.Code);
        var response = new ApiResponse<object>
        {
            Code = statusCode,
            Message = error.Message,
            Errors = error.Details != null ? new[] { error.Details } : null
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions)
        );
    }

    private static int MapHttpStatusCode(int errorCode)
    {
        return errorCode switch
        {
            (int)Enums.ErrorCategory.Unauthorized => StatusCodes.Status401Unauthorized,
            (int)Enums.ErrorCategory.ValidationError => StatusCodes.Status400BadRequest,
            (int)Enums.ErrorCategory.BusinessRuleViolation => StatusCodes.Status400BadRequest,
            (int)Enums.ErrorCategory.DatabaseUniqueKey => StatusCodes.Status409Conflict,
            (int)Enums.ErrorCategory.DatabaseForeignKey => StatusCodes.Status409Conflict,
            (int)Enums.ErrorCategory.ConcurrencyConflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}