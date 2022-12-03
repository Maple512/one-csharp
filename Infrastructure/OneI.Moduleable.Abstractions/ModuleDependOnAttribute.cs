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
    public ModuleDependOnAttribute(params Type[] denpendTypes) => DenpendedTypes = denpendTypes;

    /// <summary>
    /// 依赖项
    /// </summary>
    public Type[] DenpendedTypes { get; }

    public static IEnumerable<Type> GetAllDependedTypes(Type type) => GetAllDependedTypes(type, type.Assembly);

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
