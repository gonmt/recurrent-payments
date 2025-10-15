using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Infrastructure;

public static class UsersDbContextSeeder
{
    public static void Seed(UsersDbContext context, IHasher hasher)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(hasher);

        _ = context.Database.EnsureCreated();

        if (context.Users.Any())
        {
            return;
        }

        var id = Uuid.From(UsersSeedData.UserId);
        var email = new EmailAddress(UsersSeedData.Email);
        var fullName = new UserFullName(UsersSeedData.FullName);
        var password = UserPasswordHash.Create(UsersSeedData.Password, hasher);
        var user = User.Create(id, email, fullName, password);

        _ = context.Users.Add(user);

        _ = context.SaveChanges();
    }
}
