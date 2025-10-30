using Archetype.Api.Extensions;
using Archetype.Api.Responses;
using Archetype.Core.Users.Application;

using Microsoft.AspNetCore.Mvc;

using HttpResult = Microsoft.AspNetCore.Http.IResult;

namespace Archetype.Api.Endpoints.Users;

public class GetUserEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app) => _ = app.MapGet("/users/{id}", Handle);

    private static async Task<HttpResult> Handle([FromRoute] string id, GetUserHandler getUserHandler,
        ApiResponseWriter responses) =>
        (await getUserHandler.Find(id)).ToHttpResponse(responses);
}
