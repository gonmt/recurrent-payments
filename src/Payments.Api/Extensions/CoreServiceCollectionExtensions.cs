using System.Reflection;

using Payments.Core;
using Payments.Core.Shared.Domain;

namespace Payments.Api.Extensions;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsCore(this IServiceCollection services)
    {
        Assembly assembly = typeof(AssemblyReference).Assembly;

        foreach (Type impl in FindHandlerImplementations(assembly))
        {
            _ = services.AddScoped(impl);
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

    private static bool ImplementsIHandler(Type t) => typeof(IHandler).IsAssignableFrom(t);

    private static bool IsInCoreApplicationNamespace(string ns) =>
        ns.StartsWith("Payments.Core.", StringComparison.Ordinal) &&
        (ns.EndsWith(".Application", StringComparison.Ordinal) ||
         ns.Contains(".Application.", StringComparison.Ordinal));
}
