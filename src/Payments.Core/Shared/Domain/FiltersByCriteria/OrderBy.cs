using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Shared.Domain.FiltersByCriteria;

public sealed record OrderBy : StringValueObject
{
    public OrderBy(string value) : base(value)
    {
    }
}
