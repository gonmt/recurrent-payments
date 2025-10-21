using Bogus;

using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Infrastructure;

public static class UsersDbContextSeeder
{
    public static void Seed(UsersDbContext context, IHasher hasher)
    {

        _ = context.Database.EnsureCreated();

        if (context.Users.Any())
        {
            return;
        }

        // Mantener el usuario actual existente
        Uuid id = Uuid.From(UsersSeedData.UserId);
        EmailAddress email = new(UsersSeedData.Email);
        UserFullName fullName = new(UsersSeedData.FullName);
        UserPasswordHash password = UserPasswordHash.Create(UsersSeedData.Password, hasher);
        User user = User.Create(id, email, fullName, password);

        _ = context.Users.Add(user);

        // Generar 50 usuarios adicionales con Bogus
        List<User> users = GenerateUsers(50, hasher);
        context.Users.AddRange(users);

        _ = context.SaveChanges();
    }

    private static List<User> GenerateUsers(int count, IHasher hasher)
    {
        Faker<User> faker = new Faker<User>()
            .CustomInstantiator(f =>
            {
                Uuid id = Uuid.New();
                EmailAddress email = new(f.Internet.Email());
                UserFullName fullName = new(f.Name.FullName());
                UserPasswordHash password = UserPasswordHash.Create("Password123!", hasher);
                return User.Create(id, email, fullName, password);
            });

        return faker.Generate(count);
    }
}
