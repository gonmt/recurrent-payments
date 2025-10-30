using Archetype.Core.Shared.Domain;

namespace Archetype.Api.Responses;

public record ApiOk<T>(T Data, ApiMeta Meta) { public bool Success => true; }
public record ApiError(ApiErrorBody Error, ApiMeta Meta) { public static bool Success => false; }

public record ApiMeta(string RequestId, string? CorrelationId = null, ApiPagination? Pagination = null, string? UserId = null);
public record ApiPagination(int Page, int Size, long? Total = null, string? NextCursor = null);

public record ApiErrorBody(
    string Code,
    string Message,
    string? Details = null,
    IEnumerable<ApiFieldError>? Fields = null,
    bool Retryable = false
);

public record ApiFieldError(string Name, string Message);

public static class ApiResponses
{
    private static ApiMeta Meta(HttpContext ctx, ApiPagination? pag = null, ILogContext? logContext = null)
    {
        LogContextValues values = logContext?.Capture() ?? default;
        string? correlationId = values.CorrelationId ?? ctx.TraceIdentifier;
        string? userId = values.UserId;
        return new ApiMeta(ctx.TraceIdentifier, correlationId, pag, userId);
    }

    public static IResult OkResponse<T>(HttpContext ctx, T data,
        ApiPagination? pagination = null, ILogContext? logContext = null)
        => Results.Json(new ApiOk<T>(data, Meta(ctx, pagination, logContext)));

    public static IResult CreatedResponse<T>(HttpContext ctx, string location, T data,
        ApiPagination? pagination = null, ILogContext? logContext = null)
        => Results.Created(location, new ApiOk<T>(data, Meta(ctx, pagination, logContext)));

    public static IResult NoContentResponse() => Results.NoContent();

    public static IResult ErrorResponse(HttpContext ctx, int statusCode, string code, string message,
        string? details = null, IEnumerable<ApiFieldError>? fields = null, bool retryable = false,
        ILogContext? logContext = null)
    {
        ctx.Response.StatusCode = statusCode;
        ApiError body = new(new ApiErrorBody(code, message, details, fields, retryable), Meta(ctx, logContext: logContext));
        return Results.Json(body);
    }

    public static IResult BadRequest(HttpContext ctx, string message, string code = "BAD_REQUEST",
        string? details = null, IEnumerable<ApiFieldError>? fields = null, ILogContext? logContext = null)
        => ErrorResponse(ctx, StatusCodes.Status400BadRequest, code, message, details, fields, logContext: logContext);

    public static IResult Unauthorized(HttpContext ctx, string message = "Unauthorized.",
        string code = "UNAUTHORIZED", ILogContext? logContext = null)
        => ErrorResponse(ctx, StatusCodes.Status401Unauthorized, code, message, logContext: logContext);

    public static IResult Forbidden(HttpContext ctx, string message = "Forbidden.",
        string code = "FORBIDDEN", ILogContext? logContext = null)
        => ErrorResponse(ctx, StatusCodes.Status403Forbidden, code, message, logContext: logContext);

    public static IResult NotFound(HttpContext ctx, string message = "Not found.",
        string code = "NOT_FOUND", ILogContext? logContext = null)
        => ErrorResponse(ctx, StatusCodes.Status404NotFound, code, message, logContext: logContext);

    public static IResult Conflict(HttpContext ctx, string message, string code = "CONFLICT",
        string? details = null, ILogContext? logContext = null)
        => ErrorResponse(ctx, StatusCodes.Status409Conflict, code, message, details, logContext: logContext);

    public static IResult Unprocessable(HttpContext ctx, string message = "Validation error.",
        IEnumerable<ApiFieldError>? fields = null, string code = "VALIDATION_ERROR", ILogContext? logContext = null)
        => ErrorResponse(ctx, StatusCodes.Status422UnprocessableEntity, code, message, fields: fields, logContext: logContext);

    public static IResult RateLimited(HttpContext ctx, string message = "Too many requests.",
        string code = "RATE_LIMITED", ILogContext? logContext = null)
        => ErrorResponse(ctx, StatusCodes.Status429TooManyRequests, code, message, logContext: logContext);

    public static IResult InternalError(HttpContext ctx, string message = "An unexpected error occurred.",
        string code = "INTERNAL_ERROR", string? details = null, ILogContext? logContext = null)
        => ErrorResponse(ctx, StatusCodes.Status500InternalServerError, code, message, details, logContext: logContext);
}
