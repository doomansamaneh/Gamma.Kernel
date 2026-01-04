using System.Diagnostics.CodeAnalysis;
using Mediator;

namespace Gamma.Next.Application.Pipelines;

public sealed class MessageValidatorBehaviour<TMessage, TResponse> : MessagePreProcessor<TMessage, TResponse>
    where TMessage : IValidate
{
    protected override ValueTask Handle(TMessage message, CancellationToken cancellationToken)
    {
        if (!message.IsValid(out var validationError))
            throw new ValidationException(validationError);

        return default;
    }
}

public interface IValidate : IMessage
{
    bool IsValid([NotNullWhen(false)] out ValidationError? error);
}

public sealed record ValidationError(IEnumerable<string> Errors);

public sealed class ValidationException(ValidationError validationError)
    : Kernel.Exceptions.GammaException("Validation error")
{

    public ValidationError ValidationError { get; } = validationError;
}