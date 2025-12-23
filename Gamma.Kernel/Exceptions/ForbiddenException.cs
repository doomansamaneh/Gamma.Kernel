namespace Gamma.Kernel.Exceptions;

public sealed class ForbiddenException(string permission)
    : GammaException($"You do not have permission '{permission}' to perform this action.", 403)
{
    public string Permission { get; } = permission;
}



