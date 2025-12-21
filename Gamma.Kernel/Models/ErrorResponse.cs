namespace Gamma.Kernel.Models;

public sealed class ErrorResponse
{
    public int Code { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? Details { get; init; }

    public string? TraceId { get; init; }
}
