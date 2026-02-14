using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Queries;

public interface IGetByIdQuery<TDto> : IQuery<Result<TDto>>
{
    Guid Id { get; }
}
