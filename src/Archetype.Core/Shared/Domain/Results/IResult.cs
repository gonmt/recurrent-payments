namespace Archetype.Core.Shared.Domain.Results;

public interface IResult
{
    List<Error>? Errors { get; }

    bool IsSuccess { get; }

    bool IsError { get; }
}

public interface IResult<out TValue> : IResult
{
    TValue? Value { get; }
}
