using Archetype.Core.Shared.Domain;
using Archetype.Core.Shared.Domain.ValueObjects;
using Archetype.Core.Shared.Infrastructure;
using Archetype.Core.Users.Domain;

namespace Archetype.Core.Users.Application;

public sealed class GetUserHandler(IUserRepository userRepository) : IHandler
{
    public async Task<GetUserResponse?> Find(string id)
    {
        Uuid userId = Uuid.From(id);

        User? user = await userRepository.Find(userId);

        return user == null
            ? null
            : new GetUserResponse(user.Id, user.Email, user.FullName, user.CreatedAt.ToApplicationString());
    }
}
