namespace OneI.Moduleable.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneI.Moduleable.Infrastructure;

internal static class ModuleHelper
{
    private static ILogger? _logger;

    public static IReadOnlyList<IModuleDescriptor> LoadModules(
        IServiceCollection services,
        Type startupType,
        ILogger? logger = null)
    {
        _logger = logger;

        var modules = GetModuleDescriptors(startupType);

        if(modules.Count > 1)
        {
            modules = SortByDependency(modules, startupType);
        }

        return modules;
    }

    private static List<IModuleDescriptor> GetModuleDescriptors(Type startupType)
    {
        var modules = new List<IModuleDescriptor>();

        FillAllModules(modules, startupType);

        FillAllDependencies(modules);

        return modules;
    }

    private static List<IModuleDescriptor> SortByDependency(
        IEnumerable<IModuleDescriptor> modules,
        Type startupType)
    {
        var sortedModules = modules.SortByDependencies(m => m.Dependencies);

        sortedModules.MoveFirstItemTo(m => m.StartupType == startupType, modules.Count() - 1);

        return sortedModules;
    }

    /// <summary>
    /// 填充所有模块
    /// </summary>
    /// <param name="modules"></param>
    /// <param name="moduleType"></param>
    /// <returns></returns>
    private static void FillAllModules(ICollection<IModuleDescriptor> modules, Type moduleType)
    {
        _logger?.LogInformation("Loading module starts.");

        foreach(var item in FindAllDependedTypes(moduleType))
        {
            modules.Add(new ModuleDescriptor(item));
        }

        _logger?.LogInformation("Loading module ends.");
    }

    private static void FillAllDependencies(List<IModuleDescriptor> modules)
    {
        foreach(var module in modules)
        {
            FillDependencies(modules, module);
        }
    }

    /// <summary>
    /// 填充模块依赖
    /// </summary>
    /// <param name="allModules"></param>
    /// <param name="module"></param>
    private static void FillDependencies(
        IEnumerable<IModuleDescriptor> allModules,
        IModuleDescriptor module)
    {
        var moduleTypes = ModuleDependOnAttribute.GetAllDependedTypes(module.StartupType);

        foreach(var moduleType in moduleTypes)
        {
            var depended = allModules.FirstOrDefault(x => x.StartupType == moduleType);

            if(depended == null)
            {
                throw new ArgumentException(
                    $"Can not find a depended module {moduleType.AssemblyQualifiedName} for {module.StartupType.AssemblyQualifiedName}");
            }

            module.AddDependency(depended);
        }
    }

    private static List<Type> FindAllDependedTypes(Type moduleType)
    {
        var moduleTypes = new List<Type>();

        AddModuleWithDependenciesRecursively(moduleTypes, moduleType);

        return moduleTypes;
    }

    public static bool IsModule(Type moduleType)
    {
        return moduleType.IsClass
               && !moduleType.IsAbstract
               && !moduleType.IsGenericType;
    }

    private static void AddModuleWithDependenciesRecursively(
        ICollection<Type> moduleTypes,
        Type moduleType,
        int depth = 0)
    {
        if(moduleTypes.Contains(moduleType))
        {
            return;
        }

        moduleTypes.AddIfNotContains(moduleType);

        _logger?.LogInformation(
            string.Format(
                "{0}- {1}", new string(' ', depth),
                moduleType.FullName));

        var dependTypes = ModuleDependOnAttribute.GetAllDependedTypes(moduleType);

        foreach(var item in dependTypes)
        {
            AddModuleWithDependenciesRecursively(moduleTypes, item, depth + 1);
        }
    }
}