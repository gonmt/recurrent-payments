using Microsoft.EntityFrameworkCore;

using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Infrastructure;

public class EfUserRepository(UsersDbContext context) : IUserRepository
{
    public async Task<User?> Find(Uuid id) => await context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task Save(User user)
    {
        _ = await context.Users.AddAsync(user);
        _ = await context.SaveChangesAsync();
    }
}
