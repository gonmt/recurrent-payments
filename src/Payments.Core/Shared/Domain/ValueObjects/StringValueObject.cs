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

        string normalized = normalizer(value);
        if (normalized is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = normalized;
        validator(Value);
    }

    private static string DefaultNormalize(string v)
    {
        if (v is null)
        {
            throw new ArgumentNullException(nameof(v));
        }

        return v.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(StringValueObject vo)
    {
        return vo.Value;
    }
}
