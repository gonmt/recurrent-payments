using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;
using Payments.Tests.Shared.Domain;

namespace Payments.Tests.Users.Domain;

public class UserTests
{
    [Fact]
    public void CreateShouldInitializeUserWithGivenValues()
    {
        var hasher = new FakeHasher();
        var id = Uuid.New();
        var email = new EmailAddress("john.doe@example.com");
        var fullName = new UserFullName("John Doe");
        var password = UserPasswordHash.Create("Valid12!", hasher);

        var creationLowerBound = DateTimeOffset.UtcNow;
        var user = User.Create(id, email, fullName, password);
        var creationUpperBound = DateTimeOffset.UtcNow;

        Assert.Equal(id, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(fullName, user.FullName);
        Assert.InRange(user.CreatedAt, creationLowerBound, creationUpperBound);
    }

    [Fact]
    public void ChangePasswordShouldReplaceExistingHash()
    {
        var hasher = new FakeHasher();
        var user = BuildUser(hasher);
        var newPasswordHash = UserPasswordHash.Create("NewValid1!", hasher);

        user.ChangePassword(newPasswordHash);

        Assert.True(user.VerifyPassword("NewValid1!", hasher));
        Assert.False(user.VerifyPassword("Valid12!", hasher));
    }

    [Fact]
    public void VerifyPasswordShouldReturnHasherResult()
    {
        const string storedHash = "stored-hash";
        var creationHasher = new FakeHasher(hashFunc: _ => storedHash);
        var user = BuildUser(creationHasher);
        var verifyingHasher = new FakeHasher(verifyFunc: (plain, hash) => hash == storedHash && plain == "Valid12!");

        var result = user.VerifyPassword("Valid12!", verifyingHasher);

        Assert.True(result);
        Assert.Single(verifyingHasher.VerifyCalls);
        Assert.Equal(("Valid12!", storedHash), verifyingHasher.VerifyCalls[0]);
    }

    private static User BuildUser(FakeHasher hasher)
    {
        var id = Uuid.New();
        var email = new EmailAddress("john.doe@example.com");
        var fullName = new UserFullName("John Doe");
        var password = UserPasswordHash.Create("Valid12!", hasher);

        return User.Create(id, email, fullName, password);
    }
}
