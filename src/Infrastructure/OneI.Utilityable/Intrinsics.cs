namespace OneI;

using static InlineIL.FieldRef;
using static InlineIL.IL;
using static InlineIL.IL.Emit;
using static InlineIL.MethodRef;
using static InlineIL.TypeRef;

/// <summary>
/// 使用IL直接访问内部属性
/// <para><see cref="Unreachable"/></para>
/// <para><see cref="Add"/></para>
/// <para><see cref="Field(InlineIL.TypeRef, string)"/></para>
/// <para><see cref="Method(InlineIL.TypeRef, string)"/></para>
/// <para><see cref="Type{T}"/></para>
/// </summary>
public static class Intrinsics
{
    //public static TField GetField<TType, TField>(TType instance, string field)
    //{
    //    Push(instance);

    //    Ldfld(Field(typeof(TType), field));

    //    return Return<TField>();
    //}
}
