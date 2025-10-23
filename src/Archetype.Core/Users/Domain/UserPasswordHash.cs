using Archetype.Core.Shared.Domain;
using Archetype.Core.Shared.Domain.ValueObjects;

namespace Archetype.Core.Users.Domain;

public sealed record UserPasswordHash : StringValueObject
{
    private const int MinLength = 8;
    private const int MaxLength = 12;

    private UserPasswordHash(string value) : base(value) { }

    private static void ValidatePlainPassword(string plainPassword)
    {
        string password = plainPassword.Trim();

        if (password.Length is < MinLength or > MaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(plainPassword),
                $"Password length must be between {MinLength} and {MaxLength} characters."
            );
        }

        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter.", nameof(plainPassword));
        }

        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter.", nameof(plainPassword));
        }

        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException("Password must contain at least one digit.", nameof(plainPassword));
        }

        if (password.All(char.IsLetterOrDigit))
        {
            throw new ArgumentException("Password must contain at least one symbol.", nameof(plainPassword));
        }
    }

    public static UserPasswordHash Create(string plainPassword, IHasher hasher)
    {
        ValidatePlainPassword(plainPassword);

        string hashedPassword = hasher.Hash(plainPassword);

        return new UserPasswordHash(hashedPassword);
    }

    internal static UserPasswordHash FromHash(string hashedPassword) => new(hashedPassword);

    public bool Verify(string plainPassword, IHasher hasher) => hasher.Verify(plainPassword, Value);

    public override string ToString() => "********";
}
