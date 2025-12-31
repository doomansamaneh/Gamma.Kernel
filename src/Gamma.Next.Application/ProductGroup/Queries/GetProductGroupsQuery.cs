using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Paging;
using Gamma.Next.Application.ProductGroup.Dtos;

namespace Gamma.Next.Application.ProductGroup.Queries;

public sealed record GetProductGroupsQuery(
    PageModel<ProductGroupSearch> Page
) : IQuery<PagedResult<ProductGroupDto>>;
