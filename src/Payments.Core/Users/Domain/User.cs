using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Users.Domain;

public class User
{
    public readonly Uuid Id;
    public readonly EmailAddress Email;
    public UserFullName FullName { get; private set; }
    private UserPasswordHash _passwordHash;
    public readonly DateTimeOffset CreatedAt;

    private User(Uuid id, EmailAddress email, UserFullName fullName, UserPasswordHash passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        FullName = fullName;
        _passwordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public static User Create(Uuid id, EmailAddress email, UserFullName fullName, UserPasswordHash password)
    {
        return new User(
            id,
            email,
            fullName,
            password,
            DateTimeOffset.UtcNow
        );
    }

    public void ChangePassword(UserPasswordHash newPassword) => _passwordHash = newPassword;

    public bool VerifyPassword(string plainPassword, IHasher hasher) => _passwordHash.Verify(plainPassword, hasher);
}
