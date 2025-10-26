using System.Globalization;
using System.Security.Claims;

using Archetype.Core.Shared.Domain;

namespace Archetype.Api.Middleware;

public sealed class CorrelationContextMiddleware(RequestDelegate next)
{
    public const string CorrelationHeaderName = "X-Correlation-Id";
    public const string RequestHeaderName = "X-Request-Id";

    public async Task InvokeAsync(HttpContext context, ILogContext logContext)
    {
        string correlationId = ResolveCorrelationId(context.Request.Headers);
        string requestId = context.TraceIdentifier;
        string? userId = ResolveUserId(context.User);

        logContext.Set(new LogContextValues(correlationId, requestId, userId));

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationHeaderName] = correlationId;
            context.Response.Headers[RequestHeaderName] = requestId;
            return Task.CompletedTask;
        });

        try
        {
            await next(context);
        }
        finally
        {
            logContext.Clear();
        }
    }

    private static string ResolveCorrelationId(IHeaderDictionary headers)
    {
        if (headers.TryGetValue(CorrelationHeaderName, out Microsoft.Extensions.Primitives.StringValues headerValue))
        {
            string? provided = headerValue.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
            if (!string.IsNullOrWhiteSpace(provided))
            {
                return provided!;
            }
        }

        return Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
    }

    private static string? ResolveUserId(ClaimsPrincipal user)
    {
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.Identity?.Name;
    }
}
