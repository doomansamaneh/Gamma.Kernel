namespace Gamma.Kernel.Services;

internal sealed class MethodAuthorizationMetadata
{
    public bool RequiresAuthorization { get; init; }
    public string? Permission { get; init; }
}
