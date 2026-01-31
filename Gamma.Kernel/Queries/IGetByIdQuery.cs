using Mediator;

namespace Gamma.Kernel.Queries;

public interface IGetByIdQuery<TDto> : IQuery<TDto>
{
    Guid Id { get; }
}
