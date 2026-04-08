using AuditorPRO.Domain.Interfaces;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace AuditorPRO.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string UserId => User?.GetObjectId() ?? string.Empty;
    public string Email => User?.FindFirstValue(ClaimTypes.Email) ?? User?.FindFirstValue("preferred_username") ?? string.Empty;
    public string DisplayName => User?.GetDisplayName() ?? string.Empty;

    public IEnumerable<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
