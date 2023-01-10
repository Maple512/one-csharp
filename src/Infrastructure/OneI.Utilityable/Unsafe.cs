namespace OneI;

using DotNext.Runtime;

using SysUnsafe = System.Runtime.CompilerServices.Unsafe;

public static class Unsafe
{
    public static unsafe int GetHashCode32<T>(ref T value, int length)
    {
        return Intrinsics.GetHashCode32(SysUnsafe.AsPointer(ref value), (uint)length);
    }
}
