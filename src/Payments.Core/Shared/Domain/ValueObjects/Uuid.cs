namespace Payments.Core.Shared.Domain.ValueObjects;

public sealed record Uuid : StringValueObject
{
    private Uuid(string value)
        : base(value, validator: EnsureIsVersion7)
    {
    }

    public static Uuid New() => new(Guid.CreateVersion7().ToString());

    public static Uuid From(string id) => new(id);

    private static void EnsureIsVersion7(string v)
    {
        if (!Guid.TryParse(v, out Guid guid) || guid.Version != 7)
        {
            throw new ArgumentException("Invalid uuid", nameof(v));
        }
    }
}
