using Archetype.Api.Endpoints.Shared;
using Archetype.Api.Responses;
using Archetype.Core.Users.Application.List;

namespace Archetype.Api.Endpoints.Users;

public class UsersGetEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app) => _ = app.MapGet("/users", Handle);

    private static async Task<IResult> Handle(
        HttpContext ctx,
        ListUsersHandler listUsersHandler,
        QueryProcessor queryProcessor)
    {
        ListUsersResponse response = await listUsersHandler.Find(queryProcessor.Filters, queryProcessor.Limit, queryProcessor.Offset);

        ApiPagination? pagination = queryProcessor.Limit.HasValue
            ? new ApiPagination(((queryProcessor.Offset ?? 0) / queryProcessor.Limit.Value) + 1, queryProcessor.Limit.Value, response.Total)
            : null;

        return ApiResponses.OkResponse(ctx, response, pagination);
    }
}
