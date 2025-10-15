using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Application;

public sealed class GetUserHandler(IUserRepository userRepository) : IHandler
{
    public async Task<GetUserResponse?> Find(string id)
    {
        var userId = Uuid.From(id);

        User? user = await userRepository.Find(userId);

        return user == null ? null : new GetUserResponse(user.Id, user.Email, user.FullName, user.CreatedAt.ToLocalTime().ToString());
    }
}
