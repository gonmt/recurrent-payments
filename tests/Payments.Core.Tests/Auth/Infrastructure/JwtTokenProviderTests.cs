using System.IdentityModel.Tokens.Jwt;

using Bogus;

using Microsoft.Extensions.Options;

using Payments.Core.Auth.Infrastructure;

namespace Payments.Core.Tests.Auth.Infrastructure;

public class JwtTokenProviderTests
{
    private static readonly Faker _faker = new();

    [Fact]
    public void GenerateShouldProduceTokenWithExpectedClaims()
    {
        string signingKey = GenerateSigningKey();

        JwtTokenOptions options = new()
        {
            Issuer = _faker.Company.CompanyName(),
            Audience = _faker.Internet.DomainName(),
            SigningKey = signingKey,
            ExpirationMinutes = 5
        };

        JwtTokenProvider provider = new(Options.Create(options));

        string userId = _faker.Random.Guid().ToString();
        string email = _faker.Internet.Email();
        string fullName = _faker.Name.FullName();

        string token = provider.Generate(userId, email, fullName);

        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(options.Issuer, jwt.Issuer);
        Assert.Equal(options.Audience, jwt.Audiences.Single());
        Assert.Equal(userId, jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(email, jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(fullName, jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
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

    private static string GenerateSigningKey()
    {
        return _faker.Random.String2(32, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*");
    }
}
