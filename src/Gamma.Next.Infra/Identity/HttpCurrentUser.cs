using System.Security.Claims;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;
using Microsoft.AspNetCore.Http;

namespace Gamma.Next.Infra.Identity;

public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || user.Identity?.IsAuthenticated != true)
            return Kernel.Constants.ADMIN_ID;

        var idClaim =
            user.FindFirst(ClaimTypes.NameIdentifier)
            ?? user.FindFirst("sub")
            ?? user.FindFirst("user_id");

        return idClaim != null && Guid.TryParse(idClaim.Value, out var id)
            ? id
            : Kernel.Constants.ADMIN_ID;
    }

    public string GetUserName()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.Identity?.Name ?? Kernel.Constants.ADMIN_NAME;
    }

    public LogActorModel GetActor()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        return new LogActorModel
        {
            Id = GetUserId(),
            Name = GetUserName(),
            Ip = GetClientIp(httpContext),
            UserAgent = httpContext?.Request.Headers["User-Agent"].FirstOrDefault(),
            DeviceName = GetDeviceName(httpContext)
        };
    }

    private static string? GetClientIp(HttpContext? context)
    {
        if (context == null)
            return null;

        // Support reverse proxy / load balancer
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.ToString().Split(',').FirstOrDefault()?.Trim();

        return context.Connection.RemoteIpAddress?.ToString();
    }

    private static string? GetDeviceName(HttpContext? context)
    {
        // Optional & heuristic – keep simple
        return context?.Request.Headers["X-Device-Name"].FirstOrDefault();
    }
}
