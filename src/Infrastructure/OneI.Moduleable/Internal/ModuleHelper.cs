namespace OneI.Moduleable.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// The module helper.
/// </summary>
internal static class ModuleHelper
{
    private static ILogger? _logger;

    /// <summary>
    /// Loads the modules.
    /// </summary>
    /// <param name="startupType">The startup type.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>A list of IModuleDescriptors.</returns>
    public static IReadOnlyList<IServiceModuleDescriptor> LoadModules(
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

    /// <summary>
    /// Gets the module descriptors.
    /// </summary>
    /// <param name="startupType">The startup type.</param>
    /// <returns>A list of IModuleDescriptors.</returns>
    [Obsolete]
    private static List<IServiceModuleDescriptor> GetModuleDescriptors(Type startupType)
    {
        var modules = new List<IServiceModuleDescriptor>();

        FillAllModules(modules, startupType);

        FillAllDependencies(modules);

        return modules;
    }

    /// <summary>
    /// Sorts the by dependency.
    /// </summary>
    /// <param name="modules">The modules.</param>
    /// <param name="startupType">The startup type.</param>
    /// <returns>A list of IModuleDescriptors.</returns>
    private static List<IServiceModuleDescriptor> SortByDependency(
        IEnumerable<IServiceModuleDescriptor> modules,
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
    [Obsolete]
    private static void FillAllModules(ICollection<IServiceModuleDescriptor> modules, Type moduleType)
    {
        _logger?.LogInformation("Loading module starts.");

        foreach(var item in FindAllDependedTypes(moduleType))
        {
            modules.Add(new ModuleDescriptor(item));
        }

        _logger?.LogInformation("Loading module ends.");
    }

    /// <summary>
    /// Fills the all dependencies.
    /// </summary>
    /// <param name="modules">The modules.</param>
    private static void FillAllDependencies(List<IServiceModuleDescriptor> modules)
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
        IEnumerable<IServiceModuleDescriptor> allModules,
        IServiceModuleDescriptor module)
    {
        var moduleTypes = ServiceModuleAttribute.GetAllDependedTypes(module.StartupType);

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

    /// <summary>
    /// Finds the all depended types.
    /// </summary>
    /// <param name="moduleType">The module type.</param>
    /// <returns>A list of Types.</returns>
    private static List<Type> FindAllDependedTypes(Type moduleType)
    {
        var moduleTypes = new List<Type>();

        AddModuleWithDependenciesRecursively(moduleTypes, moduleType);

        return moduleTypes;
    }

    /// <summary>
    /// Are the module.
    /// </summary>
    /// <param name="moduleType">The module type.</param>
    /// <returns>A bool.</returns>
    public static bool IsModule(Type moduleType)
    {
        return moduleType.IsClass
               && !moduleType.IsAbstract
               && !moduleType.IsGenericType;
    }

    /// <summary>
    /// Adds the module with dependencies recursively.
    /// </summary>
    /// <param name="moduleTypes">The module types.</param>
    /// <param name="moduleType">The module type.</param>
    /// <param name="depth">The depth.</param>
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

        var dependTypes = ServiceModuleAttribute.GetAllDependedTypes(moduleType);

        foreach(var item in dependTypes)
        {
            AddModuleWithDependenciesRecursively(moduleTypes, item, depth + 1);
        }
    }
}
