namespace System.Reflection;

using OneI;

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

    /// <summary>
    /// Are the defined.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="inherit">If true, inherit.</param>
    /// <returns>A bool.</returns>

    public static bool IsDefined<T>(this MemberInfo member, bool inherit = false)
    {
        return member.IsDefined(typeof(T), inherit);
    }

    /// <summary>
    /// Gets the required attribute.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A T.</returns>

    public static T GetRequiredAttribute<T>(this Type type)
        where T : Attribute
    {
        return Check.NotNull(type.GetCustomAttribute<T>());
    }

    /// <summary>
    /// 转换为人性化可读的名称
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fullName"></param>
    /// <param name="isCompilable"></param>
    /// <returns></returns>

    public static string DisplayName(this Type type, bool fullName = true, bool isCompilable = false)
    {
        return string.Empty;
    }

    /// <summary>
    /// 转换为人性化可读的名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>

    public static string ShortDisplayName(this Type type)
    {
        return type.DisplayName(false);
    }

    public static bool IsAnonymousType(this Type type)
    {
        return type.Name.StartsWith("<>", StringComparison.Ordinal)
                && type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), inherit: false).Length > 0
                && type.Name.Contains("AnonymousType");
    }

    public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        while (typeInfo != null)
        {
            foreach (var methodInfo in typeInfo.DeclaredMethods)
            {
                yield return methodInfo;
            }

            typeInfo = typeInfo.BaseType?.GetTypeInfo();
        }
    }

    public static IEnumerable<FieldInfo> GetAllFields(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        while (typeInfo != null)
        {
            foreach (var fieldInfo in typeInfo.DeclaredFields)
            {
                yield return fieldInfo;
            }

            typeInfo = typeInfo.BaseType?.GetTypeInfo();
        }
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        while (typeInfo != null)
        {
            foreach (var propertyInfo in typeInfo.DeclaredProperties)
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
#elif NETSTANDARD2_0_OR_GREATER
internal static partial class ReflectionExtensions
{
    /// <summary>
    /// 尝试获取无参构造器
    /// </summary>
    /// <param name="type"></param>
    /// <param name="constructor"></param>
    /// <returns></returns>
    
    public static bool TryGetParameterlessConstructor(
        this Type type,
         out ConstructorInfo? constructor)
    {
        return (constructor = type.GetTypeInfo().DeclaredConstructors
            .FirstOrDefault(x => x.GetParameters().IsNullOrEmpty())) != null;
    }
}
#endif
