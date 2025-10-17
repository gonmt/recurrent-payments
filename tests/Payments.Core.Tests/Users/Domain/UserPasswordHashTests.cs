using Payments.Core.Tests.Shared.Domain;
using Payments.Core.Users.Domain;

namespace Payments.Core.Tests.Users.Domain;

public class UserPasswordHashTests
{
    [Fact]
    public void CreateWithValidPasswordShouldReturnHash()
    {
        FakeHasher hasher = new();

        UserPasswordHash passwordHash = UserPasswordHash.Create("Valid12!", hasher);

        Assert.Equal("Valid12!_HASH", passwordHash.Value);
    }

    [Theory]
    [InlineData("Short1!")]
    [InlineData("TooLongPass12!")]
    public void CreateWithInvalidLengthShouldThrowArgumentOutOfRange(string plainPassword)
    {
        FakeHasher hasher = new();

        Assert.Throws<ArgumentOutOfRangeException>(() => UserPasswordHash.Create(plainPassword, hasher));
    }

    [Theory]
    [InlineData("noupper1!")]
    [InlineData("NOLOWER1!")]
    [InlineData("NoDigits!!")]
    [InlineData("NoSymbol1")]
    public void CreateWithMissingCharacterRequirementShouldThrowArgumentException(string plainPassword)
    {
        FakeHasher hasher = new();

        Assert.Throws<ArgumentException>(() => UserPasswordHash.Create(plainPassword, hasher));
    }

    [Fact]
    public void CreateShouldAllowPasswordsTrimmedForValidation()
    {
        FakeHasher hasher = new();

        UserPasswordHash passwordHash = UserPasswordHash.Create("  Valid12!  ", hasher);

        Assert.Equal("Valid12!  _HASH", passwordHash.Value);
        Assert.Equal(("  Valid12!  ", "  Valid12!  _HASH"), hasher.HashCalls.Single());
    }

    [Fact]
    public void VerifyShouldReturnHasherResult()
    {
        FakeHasher hasher = new();
        UserPasswordHash passwordHash = UserPasswordHash.Create("Valid12!", hasher);

        bool result = passwordHash.Verify("Valid12!", hasher);

        Assert.True(result);
        Assert.Equal(("Valid12!", passwordHash.Value), hasher.VerifyCalls.Single());
    }

    [Fact]
    public void VerifyWhenHasherReturnsFalseShouldReturnFalse()
    {
        FakeHasher hasher = new(verifyFunc: (_, _) => false);
        UserPasswordHash passwordHash = UserPasswordHash.Create("Valid12!", hasher);

        Assert.False(passwordHash.Verify("Valid12!", hasher));
    }

    [Fact]
    public void ToStringShouldMaskActualValue()
    {
        FakeHasher hasher = new();
        UserPasswordHash passwordHash = UserPasswordHash.Create("Valid12!", hasher);

        Assert.Equal("********", passwordHash.ToString());
    }
}
