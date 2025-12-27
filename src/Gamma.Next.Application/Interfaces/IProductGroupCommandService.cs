using Gamma.Kernel.Abstractions;
using Gamma.Next.Application.Commands.ProductGroup;

namespace Gamma.Next.Application.Interfaces;

public interface IProductGroupCommandService : ICommandService<AddProductGroupCommand, EditProductGroupCommand, DeleteProductGroupCommand, Guid>, IApplicationService
{

}
