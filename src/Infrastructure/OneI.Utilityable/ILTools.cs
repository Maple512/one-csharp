namespace OneI;

using static InlineIL.IL;
using static InlineIL.IL.Emit;

public static unsafe class ILTools
{
    [SkipLocalsInit()]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // 删除未使用的参数
    public static byte* StackAlloc(int capacity)
#pragma warning restore IDE0060 // 删除未使用的参数
    {
        Ldarg(nameof(capacity));

        //Conv_U(); // 将位于计算堆栈顶部的值转换为 unsigned native int，然后将其扩展为 native int

        Localloc(); // 从本地动态内存池分配特定数目的字节并将第一个分配的字节的地址（瞬态指针，* 类型）推送到计算堆栈上

        return ReturnPointer<byte>();
    }
}
