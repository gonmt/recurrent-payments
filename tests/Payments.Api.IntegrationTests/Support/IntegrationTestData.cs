using Bogus;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;
using Payments.Core.Users.Infrastructure;

namespace Payments.Api.IntegrationTests.Support;

internal static class IntegrationTestData
{
    private static readonly Faker _faker = new();

    internal static async Task<(User User, string Password)> CreateUser(WebApplicationFactory<Program> factory, string? passwordOverride = null)
    {
        string password = passwordOverride ?? GenerateValidPassword();
        string email = _faker.Internet.Email();
        string fullName = _faker.Name.FullName();

        using IServiceScope scope = factory.Services.CreateScope();
        UsersDbContext context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        IHasher hasher = scope.ServiceProvider.GetRequiredService<IHasher>();

        User user = User.Create(
            Uuid.New(),
            new EmailAddress(email),
            new UserFullName(fullName),
            UserPasswordHash.Create(password, hasher));

        _ = await context.Set<User>().AddAsync(user);
        _ = await context.SaveChangesAsync();

        return (user, password);
    }

    internal static string GenerateValidPassword()
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
