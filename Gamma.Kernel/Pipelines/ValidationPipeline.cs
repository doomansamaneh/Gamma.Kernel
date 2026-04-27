using Mediator;
using FluentValidation;

namespace Gamma.Kernel.Pipelines;

public class ValidationPipeline<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken ct)
    {
        if (validators?.Any() == true)
        {
            var context = new ValidationContext<TMessage>(message);

            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next(message, ct);
    }
}


