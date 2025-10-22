using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Tests.Shared.Domain.ValueObjects;

public class EmailAddressTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("user.name+tag@sub.domain.io")]
    public void ConstructorWithValidEmailSetsValue(string email)
    {
        EmailAddress address = new(email);

        Assert.Equal(email, address.Value);
        string implicitString = address;
        Assert.Equal(email, implicitString);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("user@domain")]
    public void ConstructorWithInvalidEmailThrowsArgumentException(string email)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new EmailAddress(email));

        Assert.Contains("Invalid email address.", exception.Message);
    }
}
