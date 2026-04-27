using FluentValidation;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public record BatchActionCommandBase(List<Guid> Ids)
    : ICommand<Result<int>>;
public abstract class BatchActionCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
    where TCommand : BatchActionCommandBase
{
    protected BatchActionCommandValidatorBase()
    {
        RuleFor(x => x.Ids).NotEmpty();
    }
}
