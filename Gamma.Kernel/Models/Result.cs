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
