using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Users.Domain
{
    public interface IUserRepository
    {
        Task<User?> Find(Uuid id);
        Task Save(User user);
    }
}
