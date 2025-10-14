using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Users.Domain;

public interface IUserRepository
{
    public Task<User?> Find(Uuid id);
    public Task Save(User user);
}
