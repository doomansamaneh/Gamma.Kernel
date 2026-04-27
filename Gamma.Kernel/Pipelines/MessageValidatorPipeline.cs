using System.Diagnostics.CodeAnalysis;
using Gamma.Kernel.Exceptions;
using Mediator;

namespace Gamma.Kernel.Pipelines;

// public sealed class MessageValidatorPipeline<TMessage, TResponse> : MessagePreProcessor<TMessage, TResponse>
//     where TMessage : IValidate
// {
//     protected override ValueTask Handle(TMessage message, CancellationToken cancellationToken)
//     {
//         if (!message.IsValid(out var validationError))
//             throw new ValidationException(validationError);

//         return default;
//     }
// }

// public interface IValidate : IMessage
// {
//     bool IsValid([NotNullWhen(false)] out ValidationError? error);
// }