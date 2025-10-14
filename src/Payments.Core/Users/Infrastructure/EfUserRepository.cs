using Microsoft.EntityFrameworkCore;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Infrastructure;

public class EfUserRepository(UsersDbContext context) : IUserRepository
{
    public async Task<User?> Find(Uuid id)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task Save(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}
