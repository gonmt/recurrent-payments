using Archetype.Core.Shared.Domain.ValueObjects;

namespace Archetype.Core.Shared.Domain.FiltersByCriteria;

public sealed record FilterField : StringValueObject
{
    public FilterField(string value) : base(value)
    {
    }
}
