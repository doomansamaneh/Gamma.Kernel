using FluentValidation;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract record MergeCommandBase(
    Guid TargetId,
    List<Guid> SourceIds,
    bool DeleteSource
) : ICommand<Result<int>>;

public abstract class MergeCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
    where TCommand : MergeCommandBase
{
    protected MergeCommandValidatorBase()
    {
        RuleFor(x => x.TargetId).NotEmpty();
        RuleFor(x => x.SourceIds).NotEmpty();
    }
}

