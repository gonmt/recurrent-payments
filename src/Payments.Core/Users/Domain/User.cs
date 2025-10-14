using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Users.Domain
{
    public class User
    {
        public Uuid Id { get; private set; } = null!;
        public EmailAddress Email { get; private set; } = null!;
        public UserFullName FullName { get; private set; } = null!;
        public UserPasswordHash PasswordHash { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private set; }

        private User()
        {
            // Required by EF Core
        }

        private User(Uuid id, EmailAddress email, UserFullName fullName, UserPasswordHash passwordHash, DateTimeOffset createdAt)
        {
            Id = id;
            Email = email;
            FullName = fullName;
            PasswordHash = passwordHash;
            CreatedAt = createdAt;
        }

        public static User Create(Uuid id, EmailAddress email, UserFullName fullName, UserPasswordHash password)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(email);
            ArgumentNullException.ThrowIfNull(fullName);
            ArgumentNullException.ThrowIfNull(password);

            return new User(
                id,
                email,
                fullName,
                password,
                DateTimeOffset.UtcNow
            );
        }

        public void ChangePassword(UserPasswordHash newPassword)
        {
            ArgumentNullException.ThrowIfNull(newPassword);
            PasswordHash = newPassword;
        }

        public bool VerifyPassword(string plainPassword, IHasher hasher) => PasswordHash.Verify(plainPassword, hasher);
    }
}
