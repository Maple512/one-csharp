namespace System.Reflection;
// TODO: 使用源生成器解决反射

/// <summary>
/// The type extensions.
/// </summary>
[DebuggerStepThrough]
internal static partial class ReflectionExtensions
{
    /// <summary>
    /// The static binding flags.
    /// </summary>
    public const BindingFlags StaticBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    public static bool IsAnonymousType(this Type type)
    {
        return type.Name.StartsWith("<>", StringComparison.Ordinal)
                && type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), inherit: false).Length > 0
                && type.Name.Contains("AnonymousType");
    }

    public static IEnumerable<FieldInfo> GetAllFields(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        while(typeInfo != null)
        {
            foreach(var fieldInfo in typeInfo.DeclaredFields)
            {
                yield return fieldInfo;
            }

            typeInfo = typeInfo.BaseType?.GetTypeInfo();
        }
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        while(typeInfo != null)
        {
            foreach(var propertyInfo in typeInfo.DeclaredProperties)
            {
                yield return propertyInfo;
            }

            typeInfo = typeInfo.BaseType?.GetTypeInfo();
        }
    }
}

#if NET
[StackTraceHidden]
internal static partial class ReflectionExtensions
{
    /// <summary>
    /// 尝试获取无参构造器
    /// </summary>
    /// <param name="type"></param>
    /// <param name="constructor"></param>
    /// <returns></returns>

    public static bool TryGetParameterlessConstructor(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] this Type type,
        [NotNullWhen(true)] out ConstructorInfo? constructor)
    {
        return (constructor = type.GetTypeInfo().DeclaredConstructors
            .FirstOrDefault(x => x.GetParameters().IsNullOrEmpty())) != null;
    }
}
#endif
