using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Options;

using Payments.Core.Auth.Infrastructure;

namespace Payments.Core.Tests.Auth.Infrastructure;

public class JwtTokenProviderTests
{
    private const string SigningKey = "unit-test-signing-key-1234567890";

    [Fact]
    public void GenerateShouldProduceTokenWithExpectedClaims()
    {
        JwtTokenOptions options = new()
        {
            Issuer = "Payments",
            Audience = "Payments",
            SigningKey = SigningKey,
            ExpirationMinutes = 5
        };

        JwtTokenProvider provider = new(Options.Create(options));

        string token = provider.Generate("id-1", "user@example.com", "Jane Doe");

        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("Payments", jwt.Issuer);
        Assert.Equal("Payments", jwt.Audiences.Single());
        Assert.Equal("id-1", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("user@example.com", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal("Jane Doe", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateWithoutSigningKeyShouldThrow()
    {
        JwtTokenOptions options = new()
        {
            SigningKey = string.Empty
        };

        JwtTokenProvider provider = new(Options.Create(options));

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => provider.Generate("id", "mail", "name"));
        Assert.Equal("JWT signing key is not configured.", exception.Message);
    }
}
