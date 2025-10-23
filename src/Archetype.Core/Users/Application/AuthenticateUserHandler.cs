using Archetype.Core.Shared.Domain;
using Archetype.Core.Shared.Domain.ValueObjects;
using Archetype.Core.Users.Domain;

namespace Archetype.Core.Users.Application;

public sealed class AuthenticateUserHandler(IUserRepository userRepository, IHasher hasher) : IHandler
{
    public async Task<AuthenticateUserResponse?> Authenticate(string email, string password)
    {
        EmailAddress emailAddress = new(email);

        User? user = await userRepository.FindByEmail(emailAddress);

        if (user is null)
        {
            return null;
        }

        bool passwordMatches = user.VerifyPassword(password, hasher);

        if (!passwordMatches)
        {
            return null;
        }

        return new AuthenticateUserResponse(user.Id.Value, user.Email.Value, user.FullName.Value);
    }
}
