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

        Uuid id = Uuid.From(UsersSeedData.UserId);
        EmailAddress email = new EmailAddress(UsersSeedData.Email);
        UserFullName fullName = new UserFullName(UsersSeedData.FullName);
        UserPasswordHash password = UserPasswordHash.Create(UsersSeedData.Password, hasher);
        User user = User.Create(id, email, fullName, password);

        _ = context.Users.Add(user);

        _ = context.SaveChanges();
    }
}
