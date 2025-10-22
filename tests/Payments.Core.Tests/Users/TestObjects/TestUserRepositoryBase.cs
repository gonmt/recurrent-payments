using Payments.Core.Shared.Domain.FiltersByCriteria;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.TestObjects;

public abstract class TestUserRepositoryBase : IUserRepository
{
    protected readonly Dictionary<Uuid, User> _users = new();
    protected readonly Dictionary<EmailAddress, User> _byEmail = new();

    public virtual Task<User?> Find(Uuid id)
    {
        _users.TryGetValue(id, out User? user);
        return Task.FromResult(user);
    }

    public virtual Task<User?> FindByEmail(EmailAddress email)
    {
        _byEmail.TryGetValue(email, out User? user);
        return Task.FromResult(user);
    }

    public virtual Task<IEnumerable<User>> Matching(Criteria criteria)
    {
        IEnumerable<User> results = _users.Values;
        return Task.FromResult(results);
    }

    public virtual Task Save(User user)
    {
        _users[user.Id] = user;
        _byEmail[user.Email] = user;
        return Task.CompletedTask;
    }
}
