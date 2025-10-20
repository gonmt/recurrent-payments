using Payments.Core.Shared.Domain.FiltersByCriteria;
using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Users.Domain;

public interface IUserRepository
{
    Task<User?> Find(Uuid id);
    Task<User?> FindByEmail(EmailAddress email);
    Task<IEnumerable<User>> Matching(Criteria criteria);
    Task Save(User user);
}
