namespace OneI.Moduleable;

using System.Reflection;

/// <summary>
/// 指示标识的类是一个服务模块
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
public class ServiceModuleAttribute : Attribute
{
    public ServiceModuleAttribute(params Type[] denpendTypes)
        => DenpendedTypes = denpendTypes;

    /// <summary>
    /// 依赖项
    /// </summary>
    public Type[] DenpendedTypes { get; }

    public static IEnumerable<Type> GetAllDependedTypes(Type type)
    {
        return GetAllDependedTypes(type, type.Assembly);
    }

    public static IEnumerable<Type> GetAllDependedTypes(Type type, Assembly? assembly = null)
    {
        var attributes = type.GetCustomAttributes<ServiceModuleAttribute>();

        attributes ??= assembly?.GetCustomAttributes<ServiceModuleAttribute>();

        attributes ??= Array.Empty<ServiceModuleAttribute>();

        return attributes
               .SelectMany(x => x.DenpendedTypes)
               .Distinct();
    }
}
