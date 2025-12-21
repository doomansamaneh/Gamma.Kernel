namespace Gamma.Kernel.Exceptions;

public sealed class NotFoundException(string message) : GammaException(message, 404)
{
}

