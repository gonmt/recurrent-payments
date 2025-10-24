namespace Archetype.Core.Shared.Domain.Events;

public interface IEventSubscriber<in TDomainEvent> where TDomainEvent : DomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
