namespace Gamma.Kernel.Paging;

public class PagedResult<TItem, TSummary>
{
    public IReadOnlyList<TItem> Items { get; init; } = [];
    public TSummary? Summary { get; init; }
    public long TotalItems { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }

    public long TotalPages => PageSize > 0
        ? (long)Math.Ceiling(TotalItems / (double)PageSize)
        : 1;
}

public sealed class PagedResult<TItem> : PagedResult<TItem, EmptySummary>
{
}
