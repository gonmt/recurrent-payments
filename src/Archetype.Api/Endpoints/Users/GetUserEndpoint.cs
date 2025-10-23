using Archetype.Api.Responses;
using Archetype.Core.Users.Application;

using Microsoft.AspNetCore.Mvc;

namespace Archetype.Api.Endpoints.Users;

public class GetUserEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app) => _ = app.MapGet("/users/{id}", Handle);

    private static async Task<IResult> Handle([FromRoute] string id, HttpContext ctx, GetUserHandler getUserHandler)
    {
        GetUserResponse? user = await getUserHandler.Find(id);

        return ApiResponses.OkResponse(ctx, user);
    }
}
