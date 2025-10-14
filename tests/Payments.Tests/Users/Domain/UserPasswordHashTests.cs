using Payments.Core.Users.Domain;
using Payments.Tests.Shared.Domain;

namespace Payments.Tests.Users.Domain;

public class UserPasswordHashTests
{
    [Fact]
    public void Create_WithValidPassword_ShouldReturnHash()
    {
        var hasher = new FakeHasher();

        var passwordHash = UserPasswordHash.Create("Valid12!", hasher);

        Assert.Equal("Valid12!_HASH", passwordHash.Value);
    }

    [Theory]
    [InlineData("Short1!")]
    [InlineData("TooLongPass12!")]
    public void Create_WithInvalidLength_ShouldThrowArgumentOutOfRange(string plainPassword)
    {
        var hasher = new FakeHasher();

        Assert.Throws<ArgumentOutOfRangeException>(() => UserPasswordHash.Create(plainPassword, hasher));
    }

    [Theory]
    [InlineData("noupper1!")]
    [InlineData("NOLOWER1!")]
    [InlineData("NoDigits!!")]
    [InlineData("NoSymbol1")]
    public void Create_WithMissingCharacterRequirement_ShouldThrowArgumentException(string plainPassword)
    {
        var hasher = new FakeHasher();

        Assert.Throws<ArgumentException>(() => UserPasswordHash.Create(plainPassword, hasher));
    }

    [Fact]
    public void Create_ShouldAllowPasswordsTrimmedForValidation()
    {
        var hasher = new FakeHasher();

        var passwordHash = UserPasswordHash.Create("  Valid12!  ", hasher);

        Assert.Equal("Valid12!  _HASH", passwordHash.Value);
        Assert.Equal(("  Valid12!  ", "  Valid12!  _HASH"), hasher.HashCalls.Single());
    }

    [Fact]
    public void Verify_ShouldReturnHasherResult()
    {
        var hasher = new FakeHasher();
        var passwordHash = UserPasswordHash.Create("Valid12!", hasher);

        var result = passwordHash.Verify("Valid12!", hasher);

        Assert.True(result);
        Assert.Equal(("Valid12!", passwordHash.Value), hasher.VerifyCalls.Single());
    }

    [Fact]
    public void Verify_WhenHasherReturnsFalse_ShouldReturnFalse()
    {
        var hasher = new FakeHasher(verifyFunc: (_, _) => false);
        var passwordHash = UserPasswordHash.Create("Valid12!", hasher);

        Assert.False(passwordHash.Verify("Valid12!", hasher));
    }

    [Fact]
    public void ToString_ShouldMaskActualValue()
    {
        var hasher = new FakeHasher();
        var passwordHash = UserPasswordHash.Create("Valid12!", hasher);

        Assert.Equal("********", passwordHash.ToString());
    }
}
