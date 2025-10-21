using FluentValidation;

using Payments.Api.Responses;
using Payments.Core.Auth.Application;
using Payments.Core.Users.Application;

namespace Payments.Api.Endpoints.Auth;

public sealed class LoginEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app) => _ = app.MapPost("/auth/login", Handle);

    private static async Task<IResult> Handle(
        [Microsoft.AspNetCore.Mvc.FromBody] LoginRequest request,
        HttpContext ctx,
        IValidator<LoginRequest> validator,
        AuthenticateUserHandler authenticateUserHandler,
        GenerateTokenHandler generateTokenHandler)
    {
        FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(request);


        if (!validationResult.IsValid)
        {
            IEnumerable<ApiFieldError> fields = validationResult.Errors.Select(error => new ApiFieldError(error.PropertyName, error.ErrorMessage));
            return ApiResponses.BadRequest(ctx, "Validation error.", fields: fields);
        }

        try
        {
            AuthenticateUserResponse? user = await authenticateUserHandler.Authenticate(request.Email, request.Password);

            if (user is null)
            {
                return ApiResponses.Unauthorized(ctx, "Invalid credentials.");
            }

            GenerateTokenResponse tokenResponse = generateTokenHandler.Generate(user.Id, user.Email, user.FullName);

            return ApiResponses.OkResponse(ctx, new LoginResponse(tokenResponse.Token));
        }
        catch (ArgumentException ex)
        {
            return ApiResponses.BadRequest(ctx, ex.Message);
        }
    }
}
