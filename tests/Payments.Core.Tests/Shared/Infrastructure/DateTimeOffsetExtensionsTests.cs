using Payments.Core.Shared.Infrastructure;

namespace Payments.Core.Tests.Shared.Infrastructure;

public class DateTimeOffsetExtensionsTests
{
    [Fact]
    public void ToApplicationStringUsesRoundTripFormat()
    {
        DateTimeOffset value = new(2024, 10, 21, 15, 30, 45, TimeSpan.FromHours(-3));

        string formatted = value.ToApplicationString();

        Assert.Equal("2024-10-21T15:30:45.0000000-03:00", formatted);
    }
}
