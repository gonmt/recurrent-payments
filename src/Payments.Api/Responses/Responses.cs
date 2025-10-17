namespace Payments.Api.Responses;

public record ApiOk<T>(T Data, ApiMeta Meta) { public bool Success => true; }
public record ApiError(ApiErrorBody Error, ApiMeta Meta) { public bool Success => false; }

public record ApiMeta(string RequestId, ApiPagination? Pagination = null);
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
    private static ApiMeta Meta(HttpContext ctx, ApiPagination? pag = null)
        => new(ctx.TraceIdentifier, pag);

    public static IResult OkResponse<T>(HttpContext ctx, T data,
        ApiPagination? pagination = null)
        => Results.Json(new ApiOk<T>(data, Meta(ctx, pagination)));

    public static IResult CreatedResponse<T>(HttpContext ctx, string location, T data,
        ApiPagination? pagination = null)
        => Results.Created(location, new ApiOk<T>(data, Meta(ctx, pagination)));

    public static IResult NoContentResponse() => Results.NoContent();

    public static IResult ErrorResponse(HttpContext ctx, int statusCode, string code, string message,
        string? details = null, IEnumerable<ApiFieldError>? fields = null, bool retryable = false)
    {
        ctx.Response.StatusCode = statusCode;
        ApiError body = new(new ApiErrorBody(code, message, details, fields, retryable), Meta(ctx));
        return Results.Json(body);
    }

    public static IResult BadRequest(HttpContext ctx, string message, string code = "BAD_REQUEST",
        string? details = null, IEnumerable<ApiFieldError>? fields = null)
        => ErrorResponse(ctx, StatusCodes.Status400BadRequest, code, message, details, fields);

    public static IResult Unauthorized(HttpContext ctx, string message = "Unauthorized.",
        string code = "UNAUTHORIZED")
        => ErrorResponse(ctx, StatusCodes.Status401Unauthorized, code, message);

    public static IResult Forbidden(HttpContext ctx, string message = "Forbidden.",
        string code = "FORBIDDEN")
        => ErrorResponse(ctx, StatusCodes.Status403Forbidden, code, message);

    public static IResult NotFound(HttpContext ctx, string message = "Not found.",
        string code = "NOT_FOUND")
        => ErrorResponse(ctx, StatusCodes.Status404NotFound, code, message);

    public static IResult Conflict(HttpContext ctx, string message, string code = "CONFLICT",
        string? details = null)
        => ErrorResponse(ctx, StatusCodes.Status409Conflict, code, message, details);

    public static IResult Unprocessable(HttpContext ctx, string message = "Validation error.",
        IEnumerable<ApiFieldError>? fields = null, string code = "VALIDATION_ERROR")
        => ErrorResponse(ctx, StatusCodes.Status422UnprocessableEntity, code, message, fields: fields);

    public static IResult RateLimited(HttpContext ctx, string message = "Too many requests.",
        string code = "RATE_LIMITED")
        => ErrorResponse(ctx, StatusCodes.Status429TooManyRequests, code, message);

    public static IResult InternalError(HttpContext ctx, string message = "An unexpected error occurred.",
        string code = "INTERNAL_ERROR", string? details = null)
        => ErrorResponse(ctx, StatusCodes.Status500InternalServerError, code, message, details);
}
