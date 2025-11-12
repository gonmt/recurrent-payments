using Archetype.Core.Shared.Domain;
using Archetype.Core.Shared.Domain.ValueObjects;
using Archetype.Core.Users.Domain;

using Bogus;

using Microsoft.Extensions.DependencyInjection;

namespace Archetype.Api.IntegrationTests.Support;

internal static class IntegrationTestData
{
    private static readonly Faker _faker = new();

    internal static async Task<(User User, string Password)> CreateUser(CustomWebApplicationFactory factory, string? passwordOverride = null)
    {
        string password = passwordOverride ?? GenerateValidPassword();
        string email = _faker.Internet.Email();
        string fullName = _faker.Name.FullName();

        using IServiceScope scope = factory.Services.CreateScope();
        IHasher hasher = scope.ServiceProvider.GetRequiredService<IHasher>();
        IUserRepository repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        User user = User.Create(
            Uuid.New(),
            new EmailAddress(email),
            new UserFullName(fullName),
            UserPasswordHash.Create(password, hasher));

        await repository.Save(user);

        return (user, password);
    }

    internal static string GenerateValidPassword() => $"Test{_faker.Random.Number(10000, 99999)}!";
}
