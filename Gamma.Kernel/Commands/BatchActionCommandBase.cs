using FluentValidation;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public record BatchActionCommandBase(List<Guid> Ids)
    : ICommand<Result<int>>;
public abstract class BatchActionCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
    where TCommand : BatchActionCommandBase
{
    private const int MaxBatchSize = 100;
    protected BatchActionCommandValidatorBase()
    {
        RuleFor(x => x.Ids)
            .NotEmpty()
            .Must(x => x.Count <= MaxBatchSize)
            .WithMessage($"The number of items in a batch operation cannot exceed {MaxBatchSize}.");
    }
}
