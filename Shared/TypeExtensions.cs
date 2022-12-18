namespace System.Reflection;

using System.Linq;
using OneI;

// TODO: 使用源生成器解决反射
[StackTraceHidden]
[DebuggerStepThrough]
internal static class TypeExtensions
{
    public const BindingFlags StaticBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    [Obsolete]
    public static bool IsDefined<T>(this MemberInfo member, bool inherit = false)
    {
        return member.IsDefined(typeof(T), inherit);
    }

    [Obsolete]
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
    [Obsolete]
    public static string DisplayName(this Type type, bool fullName = true, bool isCompilable = false)
    {
        return string.Empty;
    }

    /// <summary>
    /// 转换为人性化可读的名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [Obsolete]
    public static string ShortDisplayName(this Type type)
    {
        return type.DisplayName(false);
    }


    /// <summary>
    /// 尝试获取无参构造器
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [Obsolete]
    public static bool TryGetParameterlessConstructor(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] this Type type,
        [NotNullWhen(true)] out ConstructorInfo? constructor)
    {
        return (constructor = type.GetTypeInfo().DeclaredConstructors
            .FirstOrDefault(x => x.GetParameters().IsNullOrEmpty())) != null;
    }
}
