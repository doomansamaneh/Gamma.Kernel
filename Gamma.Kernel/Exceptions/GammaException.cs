using System;

namespace Gamma.Kernel.Exceptions;

public class GammaException : Exception, IExpectedException
{
    public int StatusCode { get; }

    public GammaException(string message, int statusCode = 500)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public GammaException(string message, Exception innerException, int statusCode = 500)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
