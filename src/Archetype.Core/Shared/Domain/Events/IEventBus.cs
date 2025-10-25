namespace Archetype.Core.Shared.Domain.Events;

public interface IEventBus
{
    Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : DomainEvent;

    Task DispatchAsync<TDomainEvent>(IEnumerable<TDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        where TDomainEvent : DomainEvent;
}
