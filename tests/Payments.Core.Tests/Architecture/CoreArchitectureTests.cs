using System.Reflection;

using NetArchTest.Rules;

namespace Payments.Core.Tests.Architecture;

public class CoreArchitectureTests
{
    private const string CoreNamespace = "Payments.Core";
    private static readonly Assembly _coreAssembly = typeof(Payments.Core.AssemblyReference).Assembly;

    [Fact]
    public void ModulesShouldOnlyDependOnThemselvesOrShared()
    {
        string[] moduleRoots = GetModuleRootNamespaces();

        foreach (string moduleRoot in moduleRoots)
        {
            string[] banned = moduleRoots
                .Where(candidate => candidate != moduleRoot && candidate != $"{CoreNamespace}.Shared")
                .ToArray();

            if (banned.Length == 0)
            {
                continue;
            }

            TestResult result = Types.InAssembly(_coreAssembly)
                .That()
                .ResideInNamespaceStartingWith(moduleRoot)
                .Should()
                .NotHaveDependencyOnAny(banned)
                .GetResult();

            Assert.True(result.IsSuccessful, $"Module {moduleRoot} must not depend on other modules.");
        }
    }

    [Fact]
    public void DomainLayersShouldNotDependOnApplicationOrInfrastructure()
    {
        foreach (string module in GetModuleNames())
        {
            string domainNamespace = $"{CoreNamespace}.{module}.Domain";

            if (!HasTypesInNamespace(domainNamespace))
            {
                continue;
            }

            TestResult result = Types.InAssembly(_coreAssembly)
                .That()
                .ResideInNamespaceStartingWith(domainNamespace)
                .Should()
                .NotHaveDependencyOnAny(
                    $"{CoreNamespace}.{module}.Application",
                    $"{CoreNamespace}.{module}.Infrastructure")
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"The Domain layer of {module} cannot depend on Application or Infrastructure.");
        }
    }

    [Fact]
    public void ApplicationLayersShouldNotDependOnInfrastructure()
    {
        foreach (string module in GetModuleNames())
        {
            string applicationNamespace = $"{CoreNamespace}.{module}.Application";

            if (!HasTypesInNamespace(applicationNamespace))
            {
                continue;
            }

            TestResult result = Types.InAssembly(_coreAssembly)
                .That()
                .ResideInNamespaceStartingWith(applicationNamespace)
                .Should()
                .NotHaveDependencyOnAny($"{CoreNamespace}.{module}.Infrastructure")
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"The Application layer of {module} cannot depend on Infrastructure.");
        }
    }

    [Theory]
    [InlineData("Domain")]
    [InlineData("Application")]
    public void CoreLayersShouldOnlyDependOnBclAndCore(string layer)
    {
        foreach (string module in GetModuleNames())
        {
            string layerNamespace = $"{CoreNamespace}.{module}.{layer}";

            if (!HasTypesInNamespace(layerNamespace))
            {
                continue;
            }

            TestResult result = Types.InAssembly(_coreAssembly)
                .That()
                .ResideInNamespaceStartingWith(layerNamespace)
                .Should()
                .OnlyHaveDependenciesOn("System", CoreNamespace)
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"The {layer} layer of {module} can only depend on the C# BCL or {CoreNamespace}.");
        }
    }

    private static string[] GetModuleNames()
    {
        return _coreAssembly
            .GetTypes()
            .Select(type => type.Namespace)
            .Where(ns => ns is not null &&
                         ns.StartsWith($"{CoreNamespace}.", StringComparison.Ordinal))
            .Select(ns => ns!.Split('.', StringSplitOptions.RemoveEmptyEntries))
            .Where(parts => parts.Length >= 3)
            .Select(parts => parts[2])
            .Distinct()
            .ToArray();
    }

    private static string[] GetModuleRootNamespaces()
    {
        return GetModuleNames()
            .Select(name => $"{CoreNamespace}.{name}")
            .ToArray();
    }

    private static bool HasTypesInNamespace(string namespacePrefix)
    {
        Type[] types = _coreAssembly.GetTypes();
        return Array.Exists(
            types,
            type => type.Namespace is not null &&
                    type.Namespace.StartsWith(namespacePrefix, StringComparison.Ordinal));
    }
}
