using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;

namespace Gamma.Kernel.Paging;

public class PageModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

    public int Offset => (Page - 1) * PageSize;

    public string? SearchTerm { get; set; }

    public IReadOnlyCollection<FilterExpression> Filters { get; set; } = [];
}

public sealed class PageModel<TSearch> : PageModel
    where TSearch : ISearchModel
{
    public TSearch? Search { get; set; }
}