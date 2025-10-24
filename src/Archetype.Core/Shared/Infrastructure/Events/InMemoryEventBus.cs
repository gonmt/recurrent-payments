using Archetype.Core.Shared.Domain.Events;

using Microsoft.Extensions.DependencyInjection;

namespace Archetype.Core.Shared.Infrastructure.Events;

public class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, List<Type>> _subscribers;

    public InMemoryEventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _subscribers = [];
        DiscoverSubscribers();
    }

    private void DiscoverSubscribers()
    {
        System.Reflection.Assembly assembly = AssemblyReference.Assembly;

        List<Type> subscriberTypes = [.. assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false } &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>)))];

        foreach (Type subscriberType in subscriberTypes)
        {
            List<Type> subscriberInterfaces = subscriberType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                           i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>))
                .ToList();

            foreach (Type domainEventType in subscriberInterfaces.Select(subscriberInterface =>
                         subscriberInterface.GetGenericArguments()[0]))
            {
                if (!_subscribers.TryGetValue(domainEventType, out List<Type>? value))
                {
                    value = [];
                    _subscribers[domainEventType] = value;
                }

                value.Add(subscriberType);
            }
        }
    }

    public async Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : DomainEvent
    {
        Type eventType = typeof(TDomainEvent);

        if (_subscribers.TryGetValue(eventType, out List<Type>? subscriberTypes))
        {
            IEnumerable<Task> tasks = subscriberTypes.Select(async subscriberType =>
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                object? subscriber = scope.ServiceProvider.GetService(subscriberType);

                if (subscriber == null)
                {
                    return;
                }

                System.Reflection.MethodInfo? handleMethod = subscriberType.GetMethod("HandleAsync");
                if (handleMethod != null)
                {
                    await (Task)handleMethod.Invoke(subscriber, [domainEvent, cancellationToken])!;
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
