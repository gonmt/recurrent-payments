using System;
using System.Reflection;

using NetArchTest.Rules;

namespace Payments.Tests.Core;

public class CoreArchitectureTests
{
    private const string CoreNamespace = "Payments.Core";
    private static readonly Assembly CoreAssembly = typeof(Payments.Core.AssemblyReference).Assembly;

    [Fact]
    public void Modules_should_only_depend_on_themselves_or_shared()
    {
        var moduleRoots = GetModuleRootNamespaces();

        foreach (var moduleRoot in moduleRoots)
        {
            var banned = moduleRoots
                .Where(candidate => candidate != moduleRoot && candidate != $"{CoreNamespace}.Shared")
                .ToArray();

            if (banned.Length == 0)
            {
                continue;
            }

            var result = Types.InAssembly(CoreAssembly)
                .That()
                .ResideInNamespaceStartingWith(moduleRoot)
                .Should()
                .NotHaveDependencyOnAny(banned)
                .GetResult();

            Assert.True(result.IsSuccessful, $"Module {moduleRoot} must not depend on other modules.");
        }
    }

    [Fact]
    public void Domain_layers_should_not_depend_on_application_or_infrastructure()
    {
        foreach (var module in GetModuleNames())
        {
            var domainNamespace = $"{CoreNamespace}.{module}.Domain";

            if (!HasTypesInNamespace(domainNamespace))
            {
                continue;
            }

            var result = Types.InAssembly(CoreAssembly)
                .That()
                .ResideInNamespaceStartingWith(domainNamespace)
                .Should()
                .NotHaveDependencyOnAny($"{CoreNamespace}.{module}.Application", $"{CoreNamespace}.{module}.Infrastructure")
                .GetResult();

            Assert.True(result.IsSuccessful, $"The Domain layer of {module} cannot depend on Application or Infrastructure.");
        }
    }

    [Fact]
    public void Application_layers_should_not_depend_on_infrastructure()
    {
        foreach (var module in GetModuleNames())
        {
            var applicationNamespace = $"{CoreNamespace}.{module}.Application";

            if (!HasTypesInNamespace(applicationNamespace))
            {
                continue;
            }

            var result = Types.InAssembly(CoreAssembly)
                .That()
                .ResideInNamespaceStartingWith(applicationNamespace)
                .Should()
                .NotHaveDependencyOnAny($"{CoreNamespace}.{module}.Infrastructure")
                .GetResult();

            Assert.True(result.IsSuccessful, $"The Application layer of {module} cannot depend on Infrastructure.");
        }
    }

    [Theory]
    [InlineData("Domain")]
    [InlineData("Application")]
    public void Core_layers_should_only_depend_on_bcl_and_core(string layer)
    {
        foreach (var module in GetModuleNames())
        {
            var layerNamespace = $"{CoreNamespace}.{module}.{layer}";

            if (!HasTypesInNamespace(layerNamespace))
            {
                continue;
            }

            var result = Types.InAssembly(CoreAssembly)
                .That()
                .ResideInNamespaceStartingWith(layerNamespace)
                .Should()
                .OnlyHaveDependenciesOn("System", CoreNamespace)
                .GetResult();

            Assert.True(result.IsSuccessful, $"The {layer} layer of {module} can only depend on the C# BCL or {CoreNamespace}.");
        }
    }

    private static string[] GetModuleNames()
    {
        return CoreAssembly
            .GetTypes()
            .Select(type => type.Namespace)
            .Where(ns => ns is not null && ns.StartsWith($"{CoreNamespace}.", StringComparison.Ordinal))
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
        return CoreAssembly
            .GetTypes()
            .Any(type => type.Namespace is not null && type.Namespace.StartsWith(namespacePrefix, StringComparison.Ordinal));
    }
}
