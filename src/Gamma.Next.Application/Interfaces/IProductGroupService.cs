using Gamma.Next.Application.Commands.ProductGroup;

namespace Gamma.Next.Application.Interfaces;

public interface IProductGroupService : ICommandService<AddProductGroupCommand, EditProductGroupCommand, Guid>
{

}
