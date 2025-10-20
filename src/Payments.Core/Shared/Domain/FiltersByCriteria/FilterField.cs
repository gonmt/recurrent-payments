using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public sealed record FilterField : StringValueObject
{
    public FilterField(string value) : base(value)
    {
    }
}
