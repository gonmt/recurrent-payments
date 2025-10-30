using Archetype.Api.Extensions;
using Archetype.Api.Responses;
using Archetype.Core.Auth.Application;
using Archetype.Core.Shared.Domain.Results;
using Archetype.Core.Users.Application;

using FluentValidation;
using FluentValidation.Results;

using HttpResult = Microsoft.AspNetCore.Http.IResult;

namespace Archetype.Api.Endpoints.Auth;

public sealed class LoginEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app) => _ = app.MapPost("/auth/login", Handle);

    private static async Task<HttpResult> Handle(
        [Microsoft.AspNetCore.Mvc.FromBody] LoginRequest request,
        IValidator<LoginRequest> validator,
        AuthenticateUserHandler authenticateUserHandler,
        GenerateTokenHandler generateTokenHandler,
        ApiResponseWriter responses)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return responses.ValidationError(validationResult.Errors);
        }

        Result<AuthenticateUserResponse> authenticationResult =
            await authenticateUserHandler.Authenticate(request.Email, request.Password);

        if (authenticationResult.IsError)
        {
            return authenticationResult.ToHttpResponse(responses);
        }

        AuthenticateUserResponse user = authenticationResult.Value!;

        GenerateTokenResponse tokenResponse = generateTokenHandler.Generate(user.Id, user.Email, user.FullName);

        return responses.Ok(tokenResponse);
    }
}
