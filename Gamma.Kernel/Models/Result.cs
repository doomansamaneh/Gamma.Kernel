namespace Gamma.Kernel.Models;

public class Result<T>
{
    private Result() { }

    public bool Success { get; private set; }
    public string? Message { get; private set; }
    public T? Data { get; private set; } = default;
    public IReadOnlyList<string> Errors { get; private set; } = [];

    // Factory methods
    public static Result<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static Result<T> Fail(IEnumerable<string> errors, string? message = null)
        => new() { Success = false, Errors = [.. errors], Message = message };

    public static Result<T> Fail(string error, string? message = null) => Fail([error], message);
}

public sealed record ApiResponse<T>
{
    public int Code { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse<T> Success(T data, string message = "Success", int code = 200)
        => new() { Code = code, Message = message, Data = data };

    public static ApiResponse<T> Failure(IReadOnlyList<string> errors, string message = "Failed", int code = 400)
        => new() { Code = code, Message = message, Errors = errors };
}
