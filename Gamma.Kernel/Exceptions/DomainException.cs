namespace Gamma.Kernel.Exceptions;

public sealed class DomainException(string message) : GammaException(message, 400)
{
}

