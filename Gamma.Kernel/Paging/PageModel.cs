using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;

namespace Gamma.Kernel.Paging;

public class PageFilterModel
{
    public string? SortBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

    public string? SearchTerm { get; set; }

    public List<FilterExpression>? Filters { get; set; } = [];
}

public sealed class PageFilterModel<TSearch> : PageFilterModel
    where TSearch : ISearchModel
{
    public TSearch? Search { get; set; }
}

public class PageModel : PageFilterModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int Offset => (Page - 1) * PageSize;
}

public sealed class PageModel<TSearch> : PageModel
    where TSearch : ISearchModel
{
    public TSearch? Search { get; set; }
}