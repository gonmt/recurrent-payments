namespace Archetype.Core.Shared.Domain.Events;

public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public string EventType { get; }

    protected DomainEvent()
    {
        EventType = GetType().Name;
    }
}
