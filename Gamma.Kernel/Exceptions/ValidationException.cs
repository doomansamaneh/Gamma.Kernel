using FluentValidation.Results;

namespace Gamma.Kernel.Exceptions;

public sealed class ValidationException(ValidationFailure validationFailure)
    : Kernel.Exceptions.GammaException("Validation Failure")
{
    public ValidationFailure ValidationFailure { get; } = validationFailure;
}
