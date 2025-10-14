using System.Reflection;
using Payments.Core;
using Payments.Core.Shared.Domain;

namespace Payments.Api.Extensions;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsCore(this IServiceCollection services)
    {
        var assembly = typeof(AssemblyReference).Assembly;

        foreach (var impl in FindHandlerImplementations(assembly))
        {
            services.AddScoped(impl);
        }

        return services;
    }

    private static IEnumerable<Type> FindHandlerImplementations(Assembly asm)
    {
        return asm.GetTypes().Where(t =>
            t is { IsClass: true, IsAbstract: false } &&
            t.Namespace is not null &&
            IsInCoreApplicationNamespace(t.Namespace) &&
            ImplementsIHandler(t));
    }

    private static bool ImplementsIHandler(Type t)
    {
        return typeof(IHandler).IsAssignableFrom(t);
    }

    private static bool IsInCoreApplicationNamespace(string ns)
    {
        if (!ns.StartsWith("Payments.Core.")) return false;
        return ns.EndsWith(".Application") || ns.Contains(".Application.");
    }
}
