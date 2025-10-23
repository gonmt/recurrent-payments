using Archetype.Core.Shared.Domain.FiltersByCriteria;
using Archetype.Core.Shared.Domain.ValueObjects;

namespace Archetype.Core.Users.Domain;

public interface IUserRepository
{
    Task<User?> Find(Uuid id);
    Task<User?> FindByEmail(EmailAddress email);
    Task<IEnumerable<User>> Matching(Criteria criteria);
    Task Save(User user);
}
