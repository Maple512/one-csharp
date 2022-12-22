namespace OneI.Moduleable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// 指定模块依赖的其他模块
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
public class ModuleDependOnAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleDependOnAttribute"/> class.
    /// </summary>
    /// <param name="denpendTypes">The denpend types.</param>
    public ModuleDependOnAttribute(params Type[] denpendTypes) => DenpendedTypes = denpendTypes;

    /// <summary>
    /// 依赖项
    /// </summary>
    public Type[] DenpendedTypes { get; }

    /// <summary>
    /// Gets the all depended types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A list of Types.</returns>
    public static IEnumerable<Type> GetAllDependedTypes(Type type)
    {
        return GetAllDependedTypes(type, type.Assembly);
    }

    /// <summary>
    /// Gets the all depended types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="assembly">The assembly.</param>
    /// <returns>A list of Types.</returns>
    public static IEnumerable<Type> GetAllDependedTypes(Type type, Assembly? assembly = null)
    {
        var attributes = type.GetCustomAttributes<ModuleDependOnAttribute>();

        attributes ??= assembly?.GetCustomAttributes<ModuleDependOnAttribute>();

        attributes ??= Array.Empty<ModuleDependOnAttribute>();

        return attributes
               .SelectMany(x => x.DenpendedTypes)
               .Distinct();
    }
}
