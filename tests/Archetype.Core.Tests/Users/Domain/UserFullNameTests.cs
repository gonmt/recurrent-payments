using Archetype.Core.Users.Domain;

namespace Archetype.Core.Tests.Users.Domain;

public class UserFullNameTests
{
    [Fact]
    public void ConstructorShouldTrimValue()
    {
        UserFullName fullName = new("   Jane Doe   ");

        Assert.Equal("Jane Doe", fullName.Value);
    }

    [Theory]
    [InlineData("Jo")]
    public void ConstructorWithTooShortValueShouldThrow(string input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new UserFullName(input));
    }

    [Fact]
    public void ConstructorWithTooLongValueShouldThrow()
    {
        string input = new('a', 101);

        Assert.Throws<ArgumentOutOfRangeException>(() => new UserFullName(input));
    }
}
