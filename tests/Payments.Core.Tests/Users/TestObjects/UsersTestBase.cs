using Bogus;

using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Tests.Shared.Domain;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.TestObjects;

public abstract class UsersTestBase
{
    protected static readonly Faker Faker = new();
    protected static readonly IHasher DefaultHasher = new FakeHasher();

    protected static User CreateTestUser(
        string? email = null,
        string? fullName = null,
        string? password = null,
        Uuid? id = null) =>
        UserMother.RandomWith(
            id: id,
            email: email,
            fullName: fullName,
            password: password,
            hasher: DefaultHasher
        );

    public static string GenerateValidPassword()
    {
        List<char> characters =
        [
            Faker.Random.Char('A', 'Z'),
            Faker.Random.Char('a', 'z'),
            Faker.Random.Char('0', '9'),
            Faker.Random.ArrayElement("!@#$%^&*".ToCharArray())
        ];

        int extraCharacters = Faker.Random.Int(4, 8);
        for (int i = 0; i < extraCharacters; i++)
        {
            characters.Add(Faker.Random.ArrayElement("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*".ToCharArray()));
        }

        _ = Faker.Random.Shuffle(characters);
        return new string(characters.ToArray());
    }
}
