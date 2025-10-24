using System.Globalization;

using Archetype.Core.Shared.Domain;

namespace Archetype.Core.Shared.Infrastructure;

public class ConsoleLogger(string categoryName) : ILogger
{
    public void Debug(string message, params object[] args) => WriteLog(LogLevel.Debug, message, null, args);

    public void Information(string message, params object[] args) =>
        WriteLog(LogLevel.Information, message, null, args);

    public void Warning(string message, params object[] args) => WriteLog(LogLevel.Warning, message, null, args);

    public void Error(string message, Exception? exception = null, params object[] args) =>
        WriteLog(LogLevel.Error, message, exception, args);

    public void Critical(string message, Exception? exception = null, params object[] args) =>
        WriteLog(LogLevel.Critical, message, exception, args);

    private void WriteLog(LogLevel level, string message, Exception? exception, object[] args)
    {
        string timestamp = DateTimeOffset.Now.ToApplicationString();
        string formattedMessage = args.Length > 0 ? string.Format(CultureInfo.InvariantCulture, message, args) : message;

        string logEntry = $"[{timestamp}] [{level}] [{categoryName}] {formattedMessage}";

        if (exception != null)
        {
            logEntry += $"\nException: {exception}";
        }

        Console.WriteLine(logEntry);
    }
}
