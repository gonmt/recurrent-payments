using Archetype.Core.Shared.Domain.ValueObjects;

namespace Archetype.Core.Shared.Domain.FiltersByCriteria;

public sealed record OrderBy : StringValueObject
{
    public OrderBy(string value) : base(value)
    {
    }
}
