using Archetype.Core.Shared.Domain;

namespace Archetype.Core.Shared.Infrastructure;

public sealed class LogContext : ILogContext
{
    private sealed class ContextHolder
    {
        public string? CorrelationId { get; set; }
        public string? RequestId { get; set; }
        public string? UserId { get; set; }
    }

    private static readonly AsyncLocal<ContextHolder?> Holder = new();

    private static ContextHolder Current
    {
        get
        {
            ContextHolder? existing = Holder.Value;
            if (existing != null)
            {
                return existing;
            }

            ContextHolder created = new();
            Holder.Value = created;
            return created;
        }
    }

    public string? CorrelationId => Current.CorrelationId;
    public string? RequestId => Current.RequestId;
    public string? UserId => Current.UserId;

    public void Set(LogContextValues values)
    {
        ContextHolder ctx = Current;
        if (values.CorrelationId != null)
        {
            ctx.CorrelationId = values.CorrelationId;
        }

        if (values.RequestId != null)
        {
            ctx.RequestId = values.RequestId;
        }

        if (values.UserId != null)
        {
            ctx.UserId = values.UserId;
        }
    }

    public LogContextValues Capture()
    {
        ContextHolder ctx = Current;
        return new LogContextValues(ctx.CorrelationId, ctx.RequestId, ctx.UserId);
    }

    public void Clear()
    {
        Holder.Value = null;
    }
}
