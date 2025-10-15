namespace Payments.Core.Shared.Domain.ValueObjects;

public abstract record class StringValueObject
{
    public string Value { get; }

    protected StringValueObject(string value)
    {
        Value = Normalize(value);
        Validate(Value);
    }

    protected virtual string Normalize(string v)
        => v?.Trim() ?? throw new ArgumentNullException(nameof(v));

    protected virtual void Validate(string v) { }

    public override string ToString() => Value;

    public static implicit operator string(StringValueObject vo)
    {
        return vo.Value;
    }
}
