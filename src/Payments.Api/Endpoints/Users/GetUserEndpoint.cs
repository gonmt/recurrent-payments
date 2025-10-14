using Microsoft.AspNetCore.Mvc;
using Payments.Api.Responses;
using Payments.Core.Users.Application;

namespace Payments.Api.Endpoints.Users;

public class GetUserEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet($"/users/{{id}}", Handle);
    }

    private static async Task<IResult> Handle([FromRoute] string id, HttpContext ctx, GetUserHandler getUserHandler)
    {
        var user = await getUserHandler.Find(id);

        return ApiResponses.OkResponse(ctx, user);
    }
}
