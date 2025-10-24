namespace Archetype.Core.Shared.Domain.Results;

public readonly record struct Error
{
    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    private Error(string code, string description, int numericType)
    {
        Code = code;
        Description = description;
        NumericType = numericType;

        Type = ErrorType.Custom;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public int NumericType { get; }

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error Unexpected(string code, string description) =>
        new(code, description, ErrorType.Unexpected);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string description) =>
        new(code, description, ErrorType.Forbidden);

    public static Error Custom(int type, string code, string description) =>
        new(code, description, type);

    public bool Equals(Error other)
    {
        return Type == other.Type &&
               NumericType == other.NumericType &&
               Code == other.Code &&
               Description == other.Description;
    }

    public override int GetHashCode() => HashCode.Combine(Code, Description, Type, NumericType);
}
