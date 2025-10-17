using Bogus;

using Payments.Core.Auth.Application;
using Payments.Core.Auth.Domain;

namespace Payments.Core.Tests.Auth.Application;

public class GenerateTokenHandlerTests
{
    [Fact]
    public void GenerateShouldDelegateToTokenProvider()
    {
        FakeTokenProvider provider = new();
        GenerateTokenHandler handler = new(provider);

        Faker faker = new();
        string userId = faker.Random.Guid().ToString();
        string email = faker.Internet.Email();
        string fullName = faker.Name.FullName();

        GenerateTokenResponse response = handler.Generate(userId, email, fullName);

        Assert.Equal("generated-token", response.Token);
        Assert.Equal((userId, email, fullName), provider.LastGenerateArguments);
    }

    private sealed class FakeTokenProvider : ITokenProvider
    {
        public (string UserId, string Email, string FullName)? LastGenerateArguments { get; private set; }

        public string Generate(string userId, string email, string fullName)
        {
            LastGenerateArguments = (userId, email, fullName);
            return "generated-token";
        }
    }
}
