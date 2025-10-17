using Payments.Core.Auth.Domain;
using Payments.Core.Shared.Domain;

namespace Payments.Core.Auth.Application;

public sealed class GenerateTokenHandler(ITokenProvider tokenProvider) : IHandler
{
    public GenerateTokenResponse Generate(string userId, string email, string fullName)
    {
        string token = tokenProvider.Generate(userId, email, fullName);
        return new GenerateTokenResponse(token);
    }
}
