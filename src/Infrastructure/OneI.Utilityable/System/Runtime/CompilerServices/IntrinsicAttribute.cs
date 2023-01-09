namespace System.Runtime.CompilerServices;

/// <summary>
/// 在某些调用站点，对标记有此属性的方法的调用或对字段的引用可能会被jit内部扩展替换。
/// 运行时/编译器可能会对标记有此属性的类型进行特殊处理。
/// </summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Method
    | AttributeTargets.Constructor
    | AttributeTargets.Field, Inherited = false)]
public sealed class IntrinsicAttribute : Attribute
{
}
