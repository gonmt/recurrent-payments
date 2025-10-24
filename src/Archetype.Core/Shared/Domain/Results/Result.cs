namespace Archetype.Core.Shared.Domain.Results;

public readonly record struct Success;

public static class Result
{
    public static Success Success => default;
}

public readonly partial record struct Result<TValue> : IResult<TValue>
{
    private readonly TValue? _value = default;

    private Result(TValue value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        _value = value;
    }

    private Result(Error error)
    {
        Errors = [error];
    }

    private Result(List<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException("Cannot create an Result<TValue> from an empty collection of errors. Provide at least one error.", nameof(errors));
        }

        Errors = errors;
    }

    public bool IsSuccess => Errors is null or [];

    public bool IsError => !IsSuccess;

    public List<Error> Errors { get; } = [];

    public TValue? Value
    {
        get
        {
            if (IsError)
            {
                throw new InvalidOperationException("The Value property cannot be accessed when Errors property is not empty. Check IsSuccess or IsError before accessing the Value.");
            }

            return _value;
        }
    }

    public Error FirstError
    {
        get
        {
            if (!IsError)
            {
                throw new InvalidOperationException("The FirstError property cannot be accessed when Errors property is empty. Check IsError before accessing FirstError.");
            }

            return Errors[0];
        }
    }
}
