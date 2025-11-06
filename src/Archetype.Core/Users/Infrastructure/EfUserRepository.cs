using Archetype.Core.Shared.Domain.FiltersByCriteria;
using Archetype.Core.Shared.Domain.ValueObjects;
using Archetype.Core.Shared.Infrastructure.Persistence.EntityFramework.Criteria;
using Archetype.Core.Users.Domain;

using Microsoft.EntityFrameworkCore;

namespace Archetype.Core.Users.Infrastructure;

public class EfUserRepository(UsersDbContext context) : IUserRepository
{
    public async Task<User?> Find(Uuid id) => await context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> FindByEmail(EmailAddress email) =>
        await context.Users.FirstOrDefaultAsync(u => u.Email.Value == email.Value);

    public async Task<IEnumerable<User>> Matching(Criteria criteria) =>
        await context.Users.SearchByCriteria(criteria).ToListAsync();

    public async Task Save(User user)
    {
        _ = await context.Users.AddAsync(user);
        _ = await context.SaveChangesAsync();
    }
}
