using Gamma.Kernel.Abstractions;
using Gamma.Next.Application.ProductGroup.Commands;

namespace Gamma.Next.Application.ProductGroup;

public interface IProductGroupCommandService : ICommandService<CreateProductGroupCommand, UpdateProductGroupCommand, DeleteProductGroupCommand, Guid>, IApplicationService
{

}
