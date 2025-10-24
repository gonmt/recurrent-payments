namespace Archetype.Core.Shared.Domain.Results;

public readonly partial record struct Result<TValue>
{
    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(Error error) => new(error);

    public static implicit operator Result<TValue>(List<Error> errors) => new(errors);

    public static implicit operator Result<TValue>(Error[] errors) => new([.. errors]);
}
