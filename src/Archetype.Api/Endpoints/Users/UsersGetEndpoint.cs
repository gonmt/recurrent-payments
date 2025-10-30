using Archetype.Api.Endpoints.Shared;
using Archetype.Api.Extensions;
using Archetype.Api.Responses;
using Archetype.Core.Shared.Domain.Results;
using Archetype.Core.Users.Application.List;

using HttpResult = Microsoft.AspNetCore.Http.IResult;

namespace Archetype.Api.Endpoints.Users;

public class UsersGetEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app) => _ = app.MapGet("/users", Handle);

    private static async Task<HttpResult> Handle(
        ListUsersHandler listUsersHandler,
        QueryProcessor queryProcessor,
        ApiResponseWriter responses)
    {
        Result<ListUsersResponse> result =
            await listUsersHandler.Find(queryProcessor.Filters, queryProcessor.Limit, queryProcessor.Offset);

        ApiPagination? pagination = null;
        if (!result.IsSuccess || !queryProcessor.Limit.HasValue)
        {
            return result.ToHttpResponse(responses, pagination);
        }

        ListUsersResponse response = result.Value!;
        pagination = new ApiPagination(
            ((queryProcessor.Offset ?? 0) / queryProcessor.Limit.Value) + 1,
            queryProcessor.Limit.Value,
            response.Total);

        return result.ToHttpResponse(responses, pagination);
    }
}
