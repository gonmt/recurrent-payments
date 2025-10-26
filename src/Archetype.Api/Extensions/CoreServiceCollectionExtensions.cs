using System.Reflection;

using Archetype.Core;
using Archetype.Core.Shared.Domain;
using Archetype.Core.Shared.Domain.Events;
using Archetype.Core.Shared.Infrastructure;
using Archetype.Core.Shared.Infrastructure.Events;

namespace Archetype.Api.Extensions;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddArchetypeCore(this IServiceCollection services)
    {
        Assembly assembly = typeof(AssemblyReference).Assembly;

        foreach (Type impl in FindHandlerImplementations(assembly))
        {
            _ = services.AddScoped(impl);
        }

        foreach (Type subscriber in FindEventSubscriberImplementations(assembly))
        {
            _ = services.AddScoped(subscriber);
        }

        _ = services.AddSingleton<IEventBus, InMemoryEventBus>();
        _ = services.AddSingleton<ILogContext, LogContext>();

        return services;
    }

    private static IEnumerable<Type> FindHandlerImplementations(Assembly asm)
    {
        return asm.GetTypes().Where(t =>
            t is { IsClass: true, IsAbstract: false, Namespace: not null } &&
            IsInCoreApplicationNamespace(t.Namespace) &&
            ImplementsIHandler(t));
    }

    private static bool ImplementsIHandler(Type t) => typeof(IHandler).IsAssignableFrom(t);

    private static bool IsInCoreApplicationNamespace(string ns) =>
        ns.StartsWith("Archetype.Core.", StringComparison.Ordinal) &&
        (ns.EndsWith(".Application", StringComparison.Ordinal) ||
         ns.Contains(".Application.", StringComparison.Ordinal));

    private static IEnumerable<Type> FindEventSubscriberImplementations(Assembly assembly)
    {
        return assembly.GetTypes().Where(t =>
            t is { IsClass: true, IsAbstract: false, Namespace: not null } &&
            IsInCoreApplicationNamespace(t.Namespace) &&
            ImplementsEventSubscriber(t));
    }

    private static bool ImplementsEventSubscriber(Type type) =>
        type.GetInterfaces().Any(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>));
}
