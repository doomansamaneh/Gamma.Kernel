using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Paging;
using Gamma.Next.Application.DTOs;

namespace Gamma.Next.Application.Queries.ProductGroup;

public sealed record GetProductGroupsQuery(
    PageModel<ProductGroupSearch> Page
) : IQuery<PagedResult<ProductGroupDto>>;
