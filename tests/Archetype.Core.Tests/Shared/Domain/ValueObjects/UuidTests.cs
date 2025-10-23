using Archetype.Core.Shared.Domain.ValueObjects;

namespace Archetype.Core.Tests.Shared.Domain.ValueObjects;

public class UuidTests
{
    [Fact]
    public void NewGeneratesVersionSevenUuid()
    {
        Uuid uuid = Uuid.New();

        Guid parsed = Guid.Parse(uuid.Value);
        Assert.Equal(7, parsed.Version);
    }

    [Fact]
    public void FromWithInvalidGuidThrowsArgumentException()
    {
        const string invalidGuid = "not-a-guid";

        ArgumentException exception = Assert.Throws<ArgumentException>(() => Uuid.From(invalidGuid));

        Assert.Contains("Invalid uuid", exception.Message);
    }

    [Fact]
    public void FromWithNonVersionSevenGuidThrowsArgumentException()
    {
        string guidV4 = Guid.NewGuid().ToString();

        ArgumentException exception = Assert.Throws<ArgumentException>(() => Uuid.From(guidV4));

        Assert.Contains("Invalid uuid", exception.Message);
    }
}
