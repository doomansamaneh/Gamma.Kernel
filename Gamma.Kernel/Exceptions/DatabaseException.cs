using Gamma.Kernel.Enums;

namespace Gamma.Kernel.Exceptions;

public sealed class DatabaseException(DatabaseErrorType errorType,
    string message,
    Exception? inner = null) : Exception(message, inner), IExpectedException
{
    public DatabaseErrorType ErrorType { get; } = errorType;
}
