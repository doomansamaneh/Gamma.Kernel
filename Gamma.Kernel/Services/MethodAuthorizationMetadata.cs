namespace Gamma.Kernel.Services;

internal sealed class MethodAuthorizationMetadata
{
    public bool RequiresAuthorization { get; init; }
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
