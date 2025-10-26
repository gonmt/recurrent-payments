namespace Archetype.Core.Shared.Domain;

public interface ILogContext
{
    string? CorrelationId { get; }
    string? RequestId { get; }
    string? UserId { get; }

    void Set(LogContextValues values);
    LogContextValues Capture();
    void Clear();
}

public readonly record struct LogContextValues(string? CorrelationId, string? RequestId, string? UserId);
