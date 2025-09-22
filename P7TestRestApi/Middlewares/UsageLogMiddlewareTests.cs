using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Middlewares;

namespace P7TestRestApi.Middlewares;

public class UsageLogMiddlewareTests
{
    private static (RequestDelegate, List<string>) BuildPipeline()
    {
        var logs = new List<string>();
        var loggerFactory = LoggerFactory.Create(b => b.AddProvider(new ListLoggerProvider(logs)));
        var logger = loggerFactory.CreateLogger<UsageLogMiddleware>();

        // terminal delegate that sets a status code
        Task Terminal(HttpContext ctx)
        {
            ctx.Response.StatusCode = 204;
            return Task.CompletedTask;
        }

        var middleware = new UsageLogMiddleware(Terminal, logger);

        return (ctx => middleware.InvokeAsync(ctx), logs);
    }

    private static HttpContext CreateContext(string path, string method, bool addAuthorize, ClaimsPrincipal? user)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Request.Method = method;
        if (user != null) context.User = user;

        // Build a simple endpoint just to attach authorization metadata
        var metadataItems = new List<object>();
        if (addAuthorize)
        {
            metadataItems.Add(new Microsoft.AspNetCore.Authorization.AuthorizeAttribute());
        }
        var endpoint = new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(metadataItems), "test");
        context.SetEndpoint(endpoint);

        return context;
    }

    private static ClaimsPrincipal AuthenticatedUser(params string[] roles)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "u1"), new Claim(ClaimTypes.Name, "user1") };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    private class ListLoggerProvider : ILoggerProvider
    {
        private readonly List<string> _logs;
        public ListLoggerProvider(List<string> logs) { _logs = logs; }
        public ILogger CreateLogger(string categoryName) => new ListLogger(_logs);
        public void Dispose() { }
    }
    private class ListLogger : ILogger
    {
        private readonly List<string> _logs;
        public ListLogger(List<string> logs) { _logs = logs; }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => _logs.Add(formatter(state, exception));
    }

    [Fact]
    public async Task Logs_WhenEndpointRequiresAuthorize_AndUserAuthenticated()
    {
        var (pipeline, logs) = BuildPipeline();
        var ctx = CreateContext("/Trade", "GET", addAuthorize: true, user: AuthenticatedUser("Admin"));

        await pipeline(ctx);

        Assert.Contains(logs, m => m.Contains("Authorized action:") && m.Contains("/Trade") && m.Contains("204"));
    }

    [Fact]
    public async Task DoesNotLog_WhenEndpointIsAnonymous()
    {
        var (pipeline, logs) = BuildPipeline();
        var ctx = CreateContext("/Login", "POST", addAuthorize: false, user: AuthenticatedUser("User"));

        await pipeline(ctx);

        Assert.DoesNotContain(logs, m => m.Contains("Authorized action:"));
    }

    [Fact]
    public async Task DoesNotLog_WhenUserNotAuthenticated()
    {
        var (pipeline, logs) = BuildPipeline();
        var ctx = CreateContext("/RuleName", "GET", addAuthorize: true, user: null);

        await pipeline(ctx);

        Assert.DoesNotContain(logs, m => m.Contains("Authorized action:"));
    }
}
