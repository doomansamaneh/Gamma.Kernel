using Gamma.Kernel.Abstractions;
using Gamma.Next.Application.Commands.ProductGroup;

namespace Gamma.Next.Application.Interfaces;

public interface IProductGroupCommandService : ICommandService<CreateProductGroupCommand, UpdateProductGroupCommand, DeleteProductGroupCommand, Guid>, IApplicationService
{

}
