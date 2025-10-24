namespace Archetype.Core.Shared.Domain.Events;

public interface IEventBus
{
    Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : DomainEvent;
}
