using Bogus;

using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Tests.Shared.Domain;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.TestObjects;

public static class UserMother
{
    private static readonly Faker _faker = new();

    public static User Random() => RandomWith();

    public static User RandomWith(
        Uuid? id = null,
        string? email = null,
        string? fullName = null,
        string? password = null,
        IHasher? hasher = null)
    {
        Uuid userId = id ?? Uuid.New();
        EmailAddress userEmail = new(email ?? _faker.Internet.Email());
        UserFullName userFullName = new(fullName ?? _faker.Name.FullName());
        UserPasswordHash userPassword = UserPasswordHash.Create(
            password ?? "Valid12!",
            hasher ?? new FakeHasher()
        );

        return User.Create(userId, userEmail, userFullName, userPassword);
    }

    public static List<User> RandomMultiple(int count)
    {
        List<User> users = new();
        for (int i = 0; i < count; i++)
        {
            users.Add(Random());
        }
        return users;
    }

    public static List<User> RandomMultipleWith(
        int count,
        Uuid? id = null,
        string? emailPattern = null,
        string? fullName = null,
        string? password = null,
        IHasher? hasher = null)
    {
        List<User> users = new();
        for (int i = 0; i < count; i++)
        {
            users.Add(RandomWith(
                id: id,
                email: emailPattern != null ? $"{i}_{emailPattern}" : null,
                fullName: fullName != null ? $"{fullName}_{i}" : null,
                password: password,
                hasher: hasher
            ));
        }
        return users;
    }
}
