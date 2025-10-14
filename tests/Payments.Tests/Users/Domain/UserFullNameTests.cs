using Payments.Core.Users.Domain;

namespace Payments.Tests.Users.Domain;

public class UserFullNameTests
{
    [Fact]
    public void Constructor_ShouldTrimValue()
    {
        var fullName = new UserFullName("   Jane Doe   ");

        Assert.Equal("Jane Doe", fullName.Value);
    }

    [Theory]
    [InlineData("Jo")]
    public void Constructor_WithTooShortValue_ShouldThrow(string input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new UserFullName(input));
    }

    [Fact]
    public void Constructor_WithTooLongValue_ShouldThrow()
    {
        var input = new string('a', 101);

        Assert.Throws<ArgumentOutOfRangeException>(() => new UserFullName(input));
    }
}
