using System.Globalization;

using Archetype.Core.Shared.Domain;

namespace Archetype.Core.Shared.Infrastructure;

public class ConsoleLogger(string categoryName, ILogContext? logContext = null) : ILogger
{
    private readonly string _categoryName = categoryName;
    private readonly ILogContext? _logContext = logContext;

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

        string logEntry = $"[{timestamp}] [{level}] [{_categoryName}] {formattedMessage}";

        if (_logContext is not null)
        {
            LogContextValues snapshot = _logContext.Capture();
            string contextDetails = BuildContextDetails(snapshot);
            if (!string.IsNullOrEmpty(contextDetails))
            {
                logEntry = $"{logEntry} {contextDetails}";
            }
        }

        if (exception != null)
        {
            logEntry += $"\nException: {exception}";
        }

        Console.WriteLine(logEntry);
    }

    private static string BuildContextDetails(LogContextValues snapshot)
    {
        List<string> parts = [];

        if (!string.IsNullOrWhiteSpace(snapshot.CorrelationId))
        {
            parts.Add($"correlationId={snapshot.CorrelationId}");
        }

        if (!string.IsNullOrWhiteSpace(snapshot.RequestId))
        {
            parts.Add($"requestId={snapshot.RequestId}");
        }

        if (!string.IsNullOrWhiteSpace(snapshot.UserId))
        {
            parts.Add($"userId={snapshot.UserId}");
        }

        return parts.Count == 0 ? string.Empty : $"[{string.Join(',', parts)}]";
    }
}
