using Gamma.Kernel.Models;
using Gamma.Kernel.Paging;
using Microsoft.AspNetCore.Mvc;

namespace Gamma.Next.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResponse<T>(this Result<T> result)
    {
        return result.Success
                 ? Results.Ok(result.Data)
                 : Results.BadRequest(result.Errors);
    }

    public static IResult ToCreatedResponse<T>(this Result<T> result, string uri)
    {
        return result.Success
                ? Results.Created(uri, result.Data)
                : Results.BadRequest(result.Errors);

        // var errorResponse = ApiResponse<T>.Failure(
        //     errors: result.Errors,
        //     message: result.Message ?? "Failed",
        //     code: 400
        // );
        // return Results.BadRequest(errorResponse);
    }

    public static IResult ToNoContentResponse<T>(this Result<T> result)
    {
        if (result.Success)
        {
            return Results.NoContent();
        }

        var errorResponse = ApiResponse<T>.Failure(
            errors: result.Errors,
            message: result.Message ?? "Failed",
            code: 400
        );
        return Results.BadRequest(errorResponse);
    }

    public static IResult ToApiResponse<T>(this PagedResult<T> pagedResult)
    {
        return Results.Ok(pagedResult);
    }

    public static IResult ToApiResponse<T>(this IEnumerable<T> pagedResult)
    {
        // var response = new ApiResponse<IEnumerable<T>>
        // {
        //     Code = 200,
        //     Message = "Success",
        //     Data = pagedResult,
        //     Errors = null
        // };

        return Results.Ok(pagedResult);
    }
}
