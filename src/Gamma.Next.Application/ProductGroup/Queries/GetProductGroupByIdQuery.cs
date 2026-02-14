using Gamma.Kernel.Queries;
using Gamma.Kernel.Security;
using Gamma.Next.Application.ProductGroup.Dtos;

namespace Gamma.Next.Application.ProductGroup.Queries;

[RequiresPermission("ast.product-group.read")]
public sealed record GetProductGroupByIdQuery(
    Guid Id
) : IGetByIdQuery<ProductGroupDto>;
