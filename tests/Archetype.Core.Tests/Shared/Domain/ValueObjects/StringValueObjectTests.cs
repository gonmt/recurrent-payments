using Archetype.Core.Shared.Domain.FiltersByCriteria;
using Archetype.Core.Shared.Domain.ValueObjects;

namespace Archetype.Core.Tests.Shared.Domain.ValueObjects;

public class StringValueObjectTests
{
    private sealed record CustomStringVo : StringValueObject
    {
        public CustomStringVo(string value, Func<string, string>? normalizer = null, Action<string>? validator = null)
            : base(value, normalizer, validator)
        {
        }
    }

    [Fact]
    public void DefaultNormalizerTrimsWhitespace()
    {
        OrderBy orderBy = new("   CreatedAt   ");

        Assert.Equal("CreatedAt", orderBy.Value);
    }

    [Fact]
    public void CustomNormalizerIsApplied()
    {
        CustomStringVo vo = new("value", v => v.ToUpperInvariant());

        Assert.Equal("VALUE", vo.Value);
    }

    [Fact]
    public void ValidatorRunsAgainstNormalizedValue()
    {
        static void Validator(string val)
        {
            Assert.Equal("TEST", val);
        }

        _ = new CustomStringVo(" test ", v => v.Trim().ToUpperInvariant(), Validator);
    }
}
