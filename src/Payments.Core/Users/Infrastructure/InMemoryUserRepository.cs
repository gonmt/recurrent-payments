using System.Collections.Concurrent;
using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Infrastructure;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Uuid, User> _store = new();

    public InMemoryUserRepository(IHasher hasher)
    {
        var password = UserPasswordHash.Create("Mono8210!", hasher);
        var id = Uuid.From("0199db05-c460-72ce-891d-5892875f9663");
        var email = new EmailAddress("gon@gmail.com");
        var name = new UserFullName("Gonzalo Torres");
        
        var user = User.Create(id, email, name, password);
        
        _store.TryAdd(id, user);
    }
    
    public Task<User?> Find(Uuid id)
    {
        _store.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task Save(User user)
    {
        _store.AddOrUpdate(user.Id, user, (_, _) => user);
        return Task.CompletedTask;
    }
}
