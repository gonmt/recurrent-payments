namespace Payments.Api.Endpoints.Auth;

public sealed class LoginResponse(string token)
{
    public string Token { get; } = token;
}
