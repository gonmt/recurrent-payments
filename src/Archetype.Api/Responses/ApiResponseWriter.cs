using Archetype.Core.Shared.Domain;

using FluentValidation.Results;

namespace Archetype.Api.Responses;

public sealed class ApiResponseWriter(IHttpContextAccessor accessor, ILogContext logContext)
{
    private HttpContext HttpContext => accessor.HttpContext ??
        throw new InvalidOperationException("No active HttpContext. Ensure IHttpContextAccessor is registered.");

    public IResult Ok<T>(T data, ApiPagination? pagination = null)
        => ApiResponses.OkResponse(HttpContext, data, pagination, logContext);

    public IResult Created<T>(string location, T data, ApiPagination? pagination = null)
        => ApiResponses.CreatedResponse(HttpContext, location, data, pagination, logContext);

    public static IResult NoContent()
        => ApiResponses.NoContentResponse();

    public IResult BadRequest(string message, string code = "BAD_REQUEST", string? details = null,
        IEnumerable<ApiFieldError>? fields = null)
        => ApiResponses.BadRequest(HttpContext, message, code, details, fields, logContext);

    public IResult Unauthorized(string message = "Unauthorized.", string code = "UNAUTHORIZED")
        => ApiResponses.Unauthorized(HttpContext, message, code, logContext);

    public IResult Forbidden(string message = "Forbidden.", string code = "FORBIDDEN")
        => ApiResponses.Forbidden(HttpContext, message, code, logContext);

    public IResult NotFound(string message = "Not found.", string code = "NOT_FOUND")
        => ApiResponses.NotFound(HttpContext, message, code, logContext);

    public IResult Conflict(string message, string code = "CONFLICT", string? details = null)
        => ApiResponses.Conflict(HttpContext, message, code, details, logContext);

    public IResult ValidationError(string message = "Validation error.", IEnumerable<ApiFieldError>? fields = null,
        string code = "VALIDATION_ERROR")
        => ApiResponses.Unprocessable(HttpContext, message, fields, code, logContext);

    public IResult ValidationError(List<ValidationFailure> errors)
    {
        IEnumerable<ApiFieldError> fields = errors.Select(error => new ApiFieldError(error.PropertyName, error.ErrorMessage));

        return ApiResponses.Unprocessable(HttpContext, "Validation error.", fields, "VALIDATION_ERROR", logContext);
    }

    public IResult RateLimited(string message = "Too many requests.", string code = "RATE_LIMITED")
        => ApiResponses.RateLimited(HttpContext, message, code, logContext);

    public IResult InternalError(string message = "An unexpected error occurred.", string code = "INTERNAL_ERROR",
        string? details = null)
        => ApiResponses.InternalError(HttpContext, message, code, details, logContext);
}
