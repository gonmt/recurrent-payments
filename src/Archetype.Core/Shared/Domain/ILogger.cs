namespace Archetype.Core.Shared.Domain;

public interface ILogger
{
    void Debug(string message, params object[] args);
    void Information(string message, params object[] args);
    void Warning(string message, params object[] args);
    void Error(string message, Exception? exception = null, params object[] args);
    void Critical(string message, Exception? exception = null, params object[] args);
}

public enum LogLevel
{
    Debug = 0,
    Information = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}
