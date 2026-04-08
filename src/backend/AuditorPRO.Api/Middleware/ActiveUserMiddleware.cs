using AuditorPRO.Domain.Interfaces;
using Microsoft.Identity.Web;

namespace AuditorPRO.Api.Middleware;

public class ActiveUserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActiveUserMiddleware> _logger;

    public ActiveUserMiddleware(RequestDelegate next, ILogger<ActiveUserMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLoggerService auditLogger)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.GetObjectId() ?? string.Empty;
            var email = context.User.GetDisplayName() ?? string.Empty;

            // In production, verify user is active in Azure AD via Microsoft Graph
            // For now, trust the token's account_enabled claim
            var accountEnabled = context.User.FindFirst("account_enabled")?.Value;
            if (accountEnabled == "false")
            {
                _logger.LogWarning("Blocked inactive user {UserId} from {Path}", userId, context.Request.Path);
                await auditLogger.LogSecurityEventAsync(userId, "ACCESO_DENEGADO_USUARIO_INACTIVO", context.Request.Path, CancellationToken.None);

                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Acceso denegado. Su cuenta no está activa en el directorio corporativo.",
                    code = "USER_INACTIVE"
                });
                return;
            }
        }

        await _next(context);
    }
}

public static class ActiveUserMiddlewareExtensions
{
    public static IApplicationBuilder UseActiveUserValidation(this IApplicationBuilder app)
        => app.UseMiddleware<ActiveUserMiddleware>();
}
