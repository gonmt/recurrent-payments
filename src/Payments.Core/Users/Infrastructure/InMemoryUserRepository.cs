using System.Collections.Concurrent;

using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Infrastructure
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Uuid, User> _store = new();

        public InMemoryUserRepository(IHasher hasher)
        {
            var password = UserPasswordHash.Create(UsersSeedData.Password, hasher);
            var id = Uuid.From(UsersSeedData.UserId);
            var email = new EmailAddress(UsersSeedData.Email);
            var name = new UserFullName(UsersSeedData.FullName);

            var user = User.Create(id, email, name, password);

            _ = _store.TryAdd(id, user);
        }

        public Task<User?> Find(Uuid id)
        {
            _ = _store.TryGetValue(id, out User? user);
            return Task.FromResult(user);
        }

        public Task Save(User user)
        {
            _ = _store.AddOrUpdate(user.Id, user, (_, _) => user);
            return Task.CompletedTask;
        }
    }
}
