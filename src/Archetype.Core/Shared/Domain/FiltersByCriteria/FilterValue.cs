using Archetype.Core.Shared.Domain.ValueObjects;

namespace Archetype.Core.Shared.Domain.FiltersByCriteria;

public sealed record FilterValue : StringValueObject
{
    public FilterValue(string value) : base(value)
    {
    }
}
