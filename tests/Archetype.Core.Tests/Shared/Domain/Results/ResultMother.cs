using Archetype.Core.Shared.Domain.Results;

using Bogus;

namespace Archetype.Core.Tests.Shared.Domain.Results;

public static class ResultMother
{
    private static readonly Faker _faker = new();

    public static Result<string> Successful(string? value = null) => value ?? _faker.Lorem.Word();

    public static Result<int> Successful(int value) => value;

    public static Result<bool> SuccessfulBool(bool value = true) => value;

    public static Result<string> Failed(Error? error = null) => error ?? ErrorMother.Random();

    public static Result<string> FailedWithMultipleErrors(List<Error>? errors = null) =>
        errors ?? ErrorMother.RandomMultiple(_faker.Random.Int(2, 5));
}
