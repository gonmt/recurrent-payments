using Archetype.Core.Shared.Domain.Events;
using Archetype.Core.Shared.Infrastructure.Events;

using Microsoft.Extensions.DependencyInjection;

namespace Archetype.Core.Tests.Shared.Infrastructure.Events;

public class TestEvent(string testData) : DomainEvent
{
    public string TestData { get; } = testData;
}

public class TestEventSubscriber : IEventSubscriber<TestEvent>
{
    public static readonly List<TestEvent> ReceivedEvents = [];

    public Task HandleAsync(TestEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ReceivedEvents.Add(domainEvent);
        return Task.CompletedTask;
    }

    public static void Clear() => ReceivedEvents.Clear();
}

public class InMemoryEventBusTests
{
    private readonly InMemoryEventBus _eventBus;

    public InMemoryEventBusTests()
    {
        ServiceCollection services = new();
        services.AddScoped<TestEventSubscriber>();
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _eventBus = new InMemoryEventBus(serviceProvider);
    }

    [Fact]
    public void ConstructorShouldInitializeCorrectly()
    {
        ServiceCollection services = new();
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        InMemoryEventBus eventBus = new(serviceProvider);
        Assert.NotNull(eventBus);
    }

    [Fact]
    public async Task DispatchAsyncShouldHandleEventWithNoSubscribers()
    {
        TestEvent testEvent = new("no subscribers test");
        await _eventBus.DispatchAsync(testEvent);
        Assert.Equal("TestEvent", testEvent.EventType);
    }

    [Fact]
    public void DomainEventShouldHaveCorrectMetadata()
    {
        TestEvent testEvent = new("test metadata");
        Assert.NotEqual(Guid.Empty, testEvent.EventId);
        Assert.Equal("TestEvent", testEvent.EventType);
        Assert.True(testEvent.OccurredOn > DateTime.MinValue);
        Assert.True(testEvent.OccurredOn <= DateTime.UtcNow);
        Assert.Equal("test metadata", testEvent.TestData);
    }

    [Fact]
    public async Task DispatchAsyncShouldInvokeRealInMemoryEventBus()
    {
        TestEvent testEvent = new("real event bus test");
        await _eventBus.DispatchAsync(testEvent);

        Assert.Empty(TestEventSubscriber.ReceivedEvents);
    }
}
