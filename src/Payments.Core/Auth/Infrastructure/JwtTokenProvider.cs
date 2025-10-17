using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Payments.Core.Auth.Domain;

namespace Payments.Core.Auth.Infrastructure;

public sealed class JwtTokenProvider(IOptions<JwtTokenOptions> options) : ITokenProvider
{
    private readonly JwtTokenOptions _options = options.Value;

    public string Generate(string userId, string email, string fullName)
    {
        if (string.IsNullOrWhiteSpace(_options.SigningKey))
        {
            throw new InvalidOperationException("JWT signing key is not configured.");
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_options.SigningKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.UniqueName, fullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        DateTime utcNow = DateTime.UtcNow;
        DateTime expires = utcNow.AddMinutes(_options.ExpirationMinutes);

        JwtSecurityToken token = new(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: utcNow,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
