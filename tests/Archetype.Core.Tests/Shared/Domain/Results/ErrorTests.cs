using Archetype.Core.Shared.Domain.Results;

namespace Archetype.Core.Tests.Shared.Domain.Results;

public class ErrorTests
{
    [Fact]
    public void FailureShouldCreateErrorWithFailureType()
    {
        Error error = Error.Failure("ERR001", "Failure description");

        Assert.Equal("ERR001", error.Code);
        Assert.Equal("Failure description", error.Description);
        Assert.Equal(ErrorType.Failure, error.Type);
    }

    [Fact]
    public void UnexpectedShouldCreateErrorWithUnexpectedType()
    {
        Error error = Error.Unexpected("ERR002", "Unexpected error");

        Assert.Equal("ERR002", error.Code);
        Assert.Equal("Unexpected error", error.Description);
        Assert.Equal(ErrorType.Unexpected, error.Type);
    }

    [Fact]
    public void ValidationShouldCreateErrorWithValidationType()
    {
        Error error = Error.Validation("ERR003", "Validation failed");

        Assert.Equal("ERR003", error.Code);
        Assert.Equal("Validation failed", error.Description);
        Assert.Equal(ErrorType.Validation, error.Type);
    }

    [Fact]
    public void ConflictShouldCreateErrorWithConflictType()
    {
        Error error = Error.Conflict("ERR004", "Resource conflict");

        Assert.Equal("ERR004", error.Code);
        Assert.Equal("Resource conflict", error.Description);
        Assert.Equal(ErrorType.Conflict, error.Type);
    }

    [Fact]
    public void NotFoundShouldCreateErrorWithNotFoundType()
    {
        Error error = Error.NotFound("ERR005", "Resource not found");

        Assert.Equal("ERR005", error.Code);
        Assert.Equal("Resource not found", error.Description);
        Assert.Equal(ErrorType.NotFound, error.Type);
    }

    [Fact]
    public void UnauthorizedShouldCreateErrorWithUnauthorizedType()
    {
        Error error = Error.Unauthorized("ERR006", "Unauthorized access");

        Assert.Equal("ERR006", error.Code);
        Assert.Equal("Unauthorized access", error.Description);
        Assert.Equal(ErrorType.Unauthorized, error.Type);
    }

    [Fact]
    public void ForbiddenShouldCreateErrorWithForbiddenType()
    {
        Error error = Error.Forbidden("ERR007", "Access forbidden");

        Assert.Equal("ERR007", error.Code);
        Assert.Equal("Access forbidden", error.Description);
        Assert.Equal(ErrorType.Forbidden, error.Type);
    }

    [Fact]
    public void CustomShouldCreateErrorWithCustomType()
    {
        Error error = Error.Custom(9999, "ERR999", "Custom error");

        Assert.Equal("ERR999", error.Code);
        Assert.Equal("Custom error", error.Description);
        Assert.Equal(ErrorType.Custom, error.Type);
        Assert.Equal(9999, error.NumericType);
    }

    [Fact]
    public void CustomShouldSetNumericTypeCorrectly()
    {
        const int customType = 1234;
        Error error = Error.Custom(customType, "CODE", "Description");

        Assert.Equal(customType, error.NumericType);
        Assert.Equal(ErrorType.Custom, error.Type);
    }

    [Fact]
    public void EqualsShouldReturnTrueForEqualErrors()
    {
        Error error1 = Error.Failure("ERR001", "Description");
        Error error2 = Error.Failure("ERR001", "Description");

        Assert.True(error1.Equals(error2));
        Assert.Equal(error1, error2);
    }

    [Fact]
    public void EqualsShouldReturnFalseForDifferentCodes()
    {
        Error error1 = Error.Failure("ERR001", "Description");
        Error error2 = Error.Failure("ERR002", "Description");

        Assert.False(error1.Equals(error2));
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void EqualsShouldReturnFalseForDifferentDescriptions()
    {
        Error error1 = Error.Failure("ERR001", "Description 1");
        Error error2 = Error.Failure("ERR001", "Description 2");

        Assert.False(error1.Equals(error2));
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void EqualsShouldReturnFalseForDifferentTypes()
    {
        Error error1 = Error.Failure("ERR001", "Description");
        Error error2 = Error.Validation("ERR001", "Description");

        Assert.False(error1.Equals(error2));
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void GetHashCodeShouldReturnSameValueForEqualErrors()
    {
        Error error1 = Error.Failure("ERR001", "Description");
        Error error2 = Error.Failure("ERR001", "Description");

        Assert.Equal(error1.GetHashCode(), error2.GetHashCode());
    }

    [Fact]
    public void GetHashCodeShouldReturnDifferentValueForDifferentErrors()
    {
        Error error1 = Error.Failure("ERR001", "Description");
        Error error2 = Error.Failure("ERR002", "Description");

        Assert.NotEqual(error1.GetHashCode(), error2.GetHashCode());
    }
}
