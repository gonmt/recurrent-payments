namespace Payments.Core.Shared.Domain.ValueObjects;

public sealed record  Uuid : StringValueObject
{
    private Uuid(string value) : base(value) { }

    public static Uuid New() => new(Guid.CreateVersion7().ToString());

    public static Uuid From(string guid) => new(guid);

    protected override void Validate(string value)
    {
        if (!Guid.TryParse(value, out var guid) && guid.Version == 7)
            throw new ArgumentException("Invalid uuid");
    }
}
