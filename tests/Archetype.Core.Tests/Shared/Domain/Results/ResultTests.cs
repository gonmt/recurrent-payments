using Archetype.Core.Shared.Domain.Results;

namespace Archetype.Core.Tests.Shared.Domain.Results;

public class ResultTests
{
    [Fact]
    public void SuccessfulResultShouldHaveIsSuccessTrue()
    {
        Result<string> result = ResultMother.Successful("test value");

        Assert.True(result.IsSuccess);
        Assert.False(result.IsError);
        Assert.Empty(result.Errors);
        Assert.Equal("test value", result.Value);
    }

    [Fact]
    public void FailedResultShouldHaveIsErrorTrue()
    {
        Error error = Error.Failure("ERR001", "Test error");
        Result<string> result = ResultMother.Failed(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsError);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.Errors[0]);
        Assert.Equal(error, result.FirstError);
    }

    [Fact]
    public void FailedResultWithMultipleErrorsShouldContainAllErrors()
    {
        List<Error> errors = ErrorMother.RandomMultiple(3);
        Result<string> result = ResultMother.FailedWithMultipleErrors(errors);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsError);
        Assert.Equal(3, result.Errors.Count);
        Assert.Equal(errors, result.Errors);
        Assert.Equal(errors[0], result.FirstError);
    }

    [Fact]
    public void ValuePropertyShouldThrowExceptionWhenResultIsError()
    {
        Result<string> result = ResultMother.Failed();

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void FirstErrorPropertyShouldThrowExceptionWhenResultIsSuccess()
    {
        Result<string> result = ResultMother.Successful("test");

        Assert.Throws<InvalidOperationException>(() => result.FirstError);
    }


    [Fact]
    public void ResultFromValueShouldCreateSuccessfulResult()
    {
        const int value = 42;
        Result<int> result = value;

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void ResultFromErrorShouldCreateFailedResult()
    {
        Error error = Error.Failure("ERR001", "Test error");
        Result<int> result = error;

        Assert.True(result.IsError);
        Assert.Equal(error, result.FirstError);
    }

    [Fact]
    public void ResultFromErrorListShouldCreateFailedResult()
    {
        List<Error> errors = ErrorMother.RandomMultiple(2);
        Result<int> result = errors;

        Assert.True(result.IsError);
        Assert.Equal(errors, result.Errors);
    }

    [Fact]
    public void ResultFromErrorArrayShouldCreateFailedResult()
    {
        Error[] errors = ErrorMother.RandomMultiple(2).ToArray();
        Result<int> result = errors;

        Assert.True(result.IsError);
        Assert.Equal(errors, result.Errors);
    }

    [Fact]
    public void ResultWithComplexTypeShouldWorkCorrectly()
    {
        var user = new { Id = 1, Name = "Test User" };
        Result<object> result = user;

        Assert.True(result.IsSuccess);
        Assert.Equal(user, result.Value);
    }

    [Fact]
    public void ResultWithBoolValueShouldWorkCorrectly()
    {
        Result<bool> trueResult = ResultMother.SuccessfulBool();
        Result<bool> falseResult = ResultMother.SuccessfulBool(false);

        Assert.True(trueResult.IsSuccess);
        Assert.True(trueResult.Value);

        Assert.True(falseResult.IsSuccess);
        Assert.False(falseResult.Value);
    }

    [Fact]
    public void ResultSuccessShouldBeDefault()
    {
        Success success = Result.Success;

        Assert.True(success.Equals(default));
    }

    [Fact]
    public void ResultFromMultipleErrorOperationsShouldMaintainErrorState()
    {
        Error error1 = Error.Validation("ERR001", "First error");
        Error error2 = Error.Failure("ERR002", "Second error");
        Result<string> result = (List<Error>)[error1, error2];

        Assert.True(result.IsError);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(error1, result.Errors);
        Assert.Contains(error2, result.Errors);
        Assert.Equal(error1, result.FirstError);
    }

    [Fact]
    public void SuccessfulResultsWithSameValueShouldBeEqual()
    {
        const string value = "test";
        Result<string> result1 = value;
        Result<string> result2 = value;

        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.Equal(result1.Value, result2.Value);
        Assert.Equal(result1.IsError, result2.IsError);
        Assert.Equal(result1.Errors.Count, result2.Errors.Count);
    }
}
