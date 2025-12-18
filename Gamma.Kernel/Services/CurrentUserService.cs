using Gamma.Kernel.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Gamma.Kernel.Services;

public class CurrentUserService(IHttpContextAccessor httpContext) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContext = httpContext;

    public string GetUserName()
    {
        return _httpContext.HttpContext?.User?.Identity?.Name ?? "SYSTEM";
    }
}
