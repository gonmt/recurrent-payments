using Archetype.Core.Shared.Domain;
using Archetype.Core.Shared.Domain.Results;
using Archetype.Core.Shared.Domain.ValueObjects;
using Archetype.Core.Users.Domain;

namespace Archetype.Core.Users.Application;

public sealed class AuthenticateUserHandler(IUserRepository userRepository, IHasher hasher) : IHandler
{
    private const string InvalidCredentialsCode = "INVALID_CREDENTIALS";
    private const string InvalidCredentialsMessage = "Invalid credentials.";

    public async Task<Result<AuthenticateUserResponse>> Authenticate(string email, string password)
    {
        EmailAddress emailAddress = new(email);

        User? user = await userRepository.FindByEmail(emailAddress);

        if (user is null)
        {
            return Error.Unauthorized(InvalidCredentialsCode, InvalidCredentialsMessage);
        }

        bool passwordMatches = user.VerifyPassword(password, hasher);

        if (!passwordMatches)
        {
            return Error.Unauthorized(InvalidCredentialsCode, InvalidCredentialsMessage);
        }

        return new AuthenticateUserResponse(user.Id.Value, user.Email.Value, user.FullName.Value);
    }
}
