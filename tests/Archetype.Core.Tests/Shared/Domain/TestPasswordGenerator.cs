using Bogus;

namespace Archetype.Core.Tests.Shared.Domain;

public static class TestPasswordGenerator
{
    private static readonly Faker _faker = new();

    public static string GenerateValidPassword()
    {
        List<char> characters =
        [
            _faker.Random.Char('A', 'Z'),
            _faker.Random.Char('a', 'z'),
            _faker.Random.Char('0', '9'),
            _faker.Random.ArrayElement("!@#$%^&*".ToCharArray())
        ];

        int extraCharacters = _faker.Random.Int(4, 8);
        for (int i = 0; i < extraCharacters; i++)
        {
            characters.Add(_faker.Random.ArrayElement("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*".ToCharArray()));
        }

        _ = _faker.Random.Shuffle(characters);
        return new string(characters.ToArray());
    }
}
