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

        GenerateTokenResponse response = handler.Generate("user-1", "user@example.com", "Jane Doe");

        Assert.Equal("generated-token", response.Token);
        Assert.Equal(("user-1", "user@example.com", "Jane Doe"), provider.LastGenerateArguments);
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
