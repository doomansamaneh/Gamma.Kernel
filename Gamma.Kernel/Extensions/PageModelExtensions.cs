using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Paging;

namespace Gamma.Kernel.Extensions;

public static class PageModelExtensions
{
    public static void Normalize(this PageModel page)
    {
        if (page.Page < 1) page.Page = 1;
        if (page.PageSize <= 0) page.PageSize = 10;
        if (page.PageSize > 100) page.PageSize = 100;
    }
}