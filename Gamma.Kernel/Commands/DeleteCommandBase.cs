using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract class DeleteCommandBase
    : ICommand<Result<int>>
{
    public Guid Id { get; init; }
}


