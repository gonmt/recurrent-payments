using Archetype.Core.Shared.Domain;
using Archetype.Core.Shared.Domain.Results;
using Archetype.Core.Shared.Domain.ValueObjects;
using Archetype.Core.Shared.Infrastructure;
using Archetype.Core.Users.Domain;

namespace Archetype.Core.Users.Application;

public sealed class GetUserHandler(IUserRepository userRepository) : IHandler
{
    private const string UserNotFoundCode = "USER_NOT_FOUND";
    private const string UserNotFoundMessage = "User not found.";

    public async Task<Result<GetUserResponse>> Find(string id)
    {
        Uuid userId = Uuid.From(id);

        User? user = await userRepository.Find(userId);

        return user == null
            ? Error.NotFound(UserNotFoundCode, UserNotFoundMessage)
            : new GetUserResponse(user.Id, user.Email, user.FullName, user.CreatedAt.ToApplicationString());
    }
}
