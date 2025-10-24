namespace Archetype.Core.Shared.Domain.Results;

public enum ErrorType
{
    Failure = 0,
    Unexpected = 1,
    Validation = 2,
    Conflict = 3,
    NotFound = 4,
    Unauthorized = 5,
    Forbidden = 6,
    Custom = 7
}
