using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public sealed record FilterValue : StringValueObject
{
    public FilterValue(string value) : base(value)
    {
    }
}
