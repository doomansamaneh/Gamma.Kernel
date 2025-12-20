namespace Gamma.Kernel.Exceptions;

public class NotFoundException(string message) : GammaException(message, 404)
{
}

