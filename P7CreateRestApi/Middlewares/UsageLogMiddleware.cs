using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace P7CreateRestApi.Middlewares;

public class UsageLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UsageLogMiddleware> _logger;

    public UsageLogMiddleware(RequestDelegate next, ILogger<UsageLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Run the rest of the pipeline first so we can log the final status code
        await _next(context);

        var endpoint = context.GetEndpoint();
        var requiresAuth = endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null
                           && endpoint.Metadata.GetMetadata<IAllowAnonymous>() == null;
        var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

        if (requiresAuth && isAuthenticated)
        {
            var username = context.User.Identity?.Name
                           ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? "unknown";
            var roles = string.Join(
                ",",
                context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)
            );

            _logger.LogInformation(
                "Authorized action: {Method} {Path} by {User} roles=[{Roles}] => {StatusCode}",
                context.Request.Method,
                context.Request.Path.Value,
                username,
                roles,
                context.Response?.StatusCode
            );
        }
    }
}
