using Gamma.Kernel.Security;
using Gamma.Next.Application.ProductGroup.Dtos;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Queries;

[RequiresPermission("ast.product-group.read")]
public sealed record GetByIdProductGroupQuery(
    Guid Id
) : IQuery<ProductGroupDto>;
