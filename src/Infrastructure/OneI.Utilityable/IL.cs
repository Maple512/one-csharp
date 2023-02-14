namespace OneI;

using static InlineIL.FieldRef;
using static InlineIL.IL;
using static InlineIL.IL.Emit;
using static InlineIL.MethodRef;
using static InlineIL.TypeRef;

/// <summary>
/// 直接访问IL
/// <para><see cref="Unreachable"/></para>
/// <para><see cref="Add"/></para>
/// <para><see cref="Field(InlineIL.TypeRef, string)"/></para>
/// <para><see cref="Method(InlineIL.TypeRef, string)"/></para>
/// <para><see cref="Type{T}"/></para>
/// </summary>
public static class IL
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RuntimeTypeHandle GetTypeHandle<T>()
    {
        Ldtoken<T>();

        return Return<RuntimeTypeHandle>();
    }
}
