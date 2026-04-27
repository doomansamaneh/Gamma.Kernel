using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract record DeleteCommandBase
    : ICommand<Result<int>>
{
    public Guid Id { get; init; }
}
