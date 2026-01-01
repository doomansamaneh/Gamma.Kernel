using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using Gamma.Next.Application.ProductGroup.Dtos;

namespace Gamma.Next.Application.ProductGroup.Queries;

[RequiresPermission("ast.product-group.read")]
[RequiresPermission("ast.product-group.create")]
public sealed record GetProductGroupQuery(
    PageModel<ProductGroupSearch> Page
) : IQuery<PagedResult<ProductGroupDto>>;
