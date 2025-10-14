using Payments.Core.Shared.Domain.ValueObjects;

namespace Payments.Core.Users.Domain;

public sealed record class UserFullName : StringValueObject
{
    public UserFullName(string value) : base(value) { }

    protected override void Validate(string v)
    {
        if (v.Length is < 3 or > 100) throw new ArgumentOutOfRangeException(nameof(v));
    }
}
