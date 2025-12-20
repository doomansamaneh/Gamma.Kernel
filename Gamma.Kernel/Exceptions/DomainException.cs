namespace Gamma.Kernel.Exceptions;

public class DomainException(string message) : GammaException(message, 400)
{
}

