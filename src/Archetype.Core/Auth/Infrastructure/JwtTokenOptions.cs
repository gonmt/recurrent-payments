namespace Archetype.Core.Auth.Infrastructure;

public sealed class JwtTokenOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SigningKey { get; init; } = string.Empty;
    public int ExpirationMinutes { get; init; } = 60;
}
