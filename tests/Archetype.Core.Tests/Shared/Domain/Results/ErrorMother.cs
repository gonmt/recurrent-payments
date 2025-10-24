using Archetype.Core.Shared.Domain.Results;

using Bogus;

namespace Archetype.Core.Tests.Shared.Domain.Results;

public static class ErrorMother
{
    private static readonly Faker _faker = new();

    public static Error Random()
    {
        string code = _faker.Random.String2(5);
        string description = _faker.Lorem.Sentence();
        ErrorType[] types = Enum.GetValues<ErrorType>();
        ErrorType type = _faker.PickRandom(types);

        return type switch
        {
            ErrorType.Failure => Error.Failure(code, description),
            ErrorType.Unexpected => Error.Unexpected(code, description),
            ErrorType.Validation => Error.Validation(code, description),
            ErrorType.Conflict => Error.Conflict(code, description),
            ErrorType.NotFound => Error.NotFound(code, description),
            ErrorType.Unauthorized => Error.Unauthorized(code, description),
            ErrorType.Forbidden => Error.Forbidden(code, description),
            ErrorType.Custom => Error.Custom(_faker.Random.Int(1000, 9999), code, description),
            _ => Error.Failure(code, description)
        };
    }

    public static List<Error> RandomMultiple(int count)
    {
        List<Error> errors = new();
        for (int i = 0; i < count; i++)
        {
            errors.Add(Random());
        }
        return errors;
    }
}
