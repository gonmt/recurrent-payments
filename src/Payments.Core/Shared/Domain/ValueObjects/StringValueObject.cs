namespace Payments.Core.Shared.Domain.ValueObjects;

public abstract record class StringValueObject
{
    public string Value { get; }

    protected StringValueObject(
        string value,
        Func<string, string>? normalizer = null,
        Action<string>? validator = null)
    {
        normalizer ??= DefaultNormalize;
        validator ??= static _ => { };

        Value = normalizer(value);
        validator(Value);
    }

    private static string DefaultNormalize(string v) => v.Trim();

    public override string ToString() => Value;

    public static implicit operator string(StringValueObject vo)
    {
        return vo.Value;
    }
}
