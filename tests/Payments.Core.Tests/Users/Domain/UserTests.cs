using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Tests.Shared.Domain;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.Domain;

public class UserTests
{
    [Fact]
    public void CreateShouldInitializeUserWithGivenValues()
    {
        FakeHasher hasher = new FakeHasher();
        Uuid id = Uuid.New();
        EmailAddress email = new EmailAddress("john.doe@example.com");
        UserFullName fullName = new UserFullName("John Doe");
        UserPasswordHash password = UserPasswordHash.Create("Valid12!", hasher);

        DateTimeOffset creationLowerBound = DateTimeOffset.UtcNow;
        User user = User.Create(id, email, fullName, password);
        DateTimeOffset creationUpperBound = DateTimeOffset.UtcNow;

        Assert.Equal(id, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(fullName, user.FullName);
        Assert.InRange(user.CreatedAt, creationLowerBound, creationUpperBound);
    }

    [Fact]
    public void ChangePasswordShouldReplaceExistingHash()
    {
        FakeHasher hasher = new FakeHasher();
        User user = BuildUser(hasher);
        UserPasswordHash newPasswordHash = UserPasswordHash.Create("NewValid1!", hasher);

        user.ChangePassword(newPasswordHash);

        Assert.True(user.VerifyPassword("NewValid1!", hasher));
        Assert.False(user.VerifyPassword("Valid12!", hasher));
    }

    [Fact]
    public void VerifyPasswordShouldReturnHasherResult()
    {
        const string storedHash = "stored-hash";
        FakeHasher creationHasher = new FakeHasher(hashFunc: _ => storedHash);
        User user = BuildUser(creationHasher);
        FakeHasher verifyingHasher = new FakeHasher(verifyFunc: (plain, hash) => hash == storedHash && plain == "Valid12!");

        bool result = user.VerifyPassword("Valid12!", verifyingHasher);

        Assert.True(result);
        Assert.Single(verifyingHasher.VerifyCalls);
        Assert.Equal(("Valid12!", storedHash), verifyingHasher.VerifyCalls[0]);
    }

    private static User BuildUser(FakeHasher hasher)
    {
        Uuid id = Uuid.New();
        EmailAddress email = new EmailAddress("john.doe@example.com");
        UserFullName fullName = new UserFullName("John Doe");
        UserPasswordHash password = UserPasswordHash.Create("Valid12!", hasher);

        return User.Create(id, email, fullName, password);
    }
}
