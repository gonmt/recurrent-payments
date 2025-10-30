using Archetype.Api.Responses;
using Archetype.Core.Shared.Domain.Results;

using HttpResult = Microsoft.AspNetCore.Http.IResult;

namespace Archetype.Api.Extensions;

public static class ResultExtensions
{
    public static HttpResult ToHttpResponse<TValue>(this Result<TValue> result, ApiResponseWriter responses,
        ApiPagination? pagination = null)
    {
        return result.IsSuccess
            ? MapSuccess(result, responses, pagination)
            : MapError(result, responses);
    }

    private static HttpResult MapSuccess<TValue>(Result<TValue> result, ApiResponseWriter responses,
        ApiPagination? pagination)
    {
        object? value = result.Value;
        if (value is Success)
        {
            return ApiResponseWriter.NoContent();
        }

        return responses.Ok((TValue)value!, pagination);
    }

    private static HttpResult MapError<TValue>(Result<TValue> result, ApiResponseWriter responses)
    {
        Error error = result.FirstError;

        return error.Type switch
        {
            ErrorType.Validation =>
                responses.ValidationError(error.Description, BuildFieldErrors(result.Errors), error.Code),
            ErrorType.NotFound => responses.NotFound(error.Description, error.Code),
            ErrorType.Conflict => responses.Conflict(error.Description, error.Code),
            ErrorType.Unauthorized => responses.Unauthorized(error.Description, error.Code),
            ErrorType.Forbidden => responses.Forbidden(error.Description, error.Code),
            ErrorType.Failure => responses.BadRequest(error.Description, error.Code),
            ErrorType.Custom when error.NumericType == StatusCodes.Status400BadRequest =>
                responses.BadRequest(error.Description, error.Code),
            ErrorType.Custom when error.NumericType == StatusCodes.Status404NotFound =>
                responses.NotFound(error.Description, error.Code),
            ErrorType.Custom when error.NumericType == StatusCodes.Status409Conflict =>
                responses.Conflict(error.Description, error.Code),
            _ => responses.InternalError(error.Description, error.Code)
        };
    }

    private static IEnumerable<ApiFieldError> BuildFieldErrors(List<Error>? errors)
    {
        if (errors == null || errors.Count == 0)
        {
            return Enumerable.Empty<ApiFieldError>();
        }

        return errors.Select(e => new ApiFieldError(e.Code, e.Description));
    }
}
