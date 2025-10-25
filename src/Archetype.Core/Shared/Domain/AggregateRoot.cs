using Archetype.Core.Shared.Domain.Events;

namespace Archetype.Core.Shared.Domain;

public abstract class AggregateRoot<TId>
{
    private readonly List<DomainEvent> _domainEvents = new();

    public TId Id { get; protected set; } = default!;

    public IReadOnlyCollection<DomainEvent> GetDomainEvents()
    {
        return _domainEvents.AsReadOnly();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
