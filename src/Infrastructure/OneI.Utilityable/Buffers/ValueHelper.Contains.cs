namespace OneI.Buffers;

using System.Numerics;
using System.Runtime.Intrinsics;

internal static partial class ValueHelper
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static bool ContainsValueType<T>(ref T searchSpace, T value, int length) where T : struct, INumber<T>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<T>.Count)
        {
            nuint num = 0u;
            while(length >= 8)
            {
                length -= 8;
                if(Unsafe.Add(ref searchSpace, num) == value || Unsafe.Add(ref searchSpace, num + 1) == value || Unsafe.Add(ref searchSpace, num + 2) == value || Unsafe.Add(ref searchSpace, num + 3) == value || Unsafe.Add(ref searchSpace, num + 4) == value || Unsafe.Add(ref searchSpace, num + 5) == value || Unsafe.Add(ref searchSpace, num + 6) == value || Unsafe.Add(ref searchSpace, num + 7) == value)
                {
                    return true;
                }

                num += 8;
            }

            if(length >= 4)
            {
                length -= 4;
                if(Unsafe.Add(ref searchSpace, num) == value || Unsafe.Add(ref searchSpace, num + 1) == value || Unsafe.Add(ref searchSpace, num + 2) == value || Unsafe.Add(ref searchSpace, num + 3) == value)
                {
                    return true;
                }

                num += 4;
            }

            while(length > 0)
            {
                length--;
                if(Unsafe.Add(ref searchSpace, num) == value)
                {
                    return true;
                }

                num++;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<T>.Count)
        {
            var left = Vector256.Create(value);
            ref var reference = ref searchSpace;
            ref var reference2 = ref Unsafe.Add(ref searchSpace, length - Vector256<T>.Count);
            do
            {
                var vector = Vector256.Equals(left, Vector256.LoadUnsafe(ref reference));
                if(vector == Vector256<T>.Zero)
                {
                    reference = ref Unsafe.Add(ref reference, Vector256<T>.Count);
                    continue;
                }

                return true;
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference, ref reference2));
            if((uint)length % Vector256<T>.Count != 0L)
            {
                var vector = Vector256.Equals(left, Vector256.LoadUnsafe(ref reference2));
                if(vector != Vector256<T>.Zero)
                {
                    return true;
                }
            }
        }
        else
        {
            var left2 = Vector128.Create(value);
            ref var reference3 = ref searchSpace;
            ref var reference4 = ref Unsafe.Add(ref searchSpace, length - Vector128<T>.Count);
            do
            {
                var vector2 = Vector128.Equals(left2, Vector128.LoadUnsafe(ref reference3));
                if(vector2 == Vector128<T>.Zero)
                {
                    reference3 = ref Unsafe.Add(ref reference3, Vector128<T>.Count);
                    continue;
                }

                return true;
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference3, ref reference4));
            if((uint)length % Vector128<T>.Count != 0L)
            {
                var vector2 = Vector128.Equals(left2, Vector128.LoadUnsafe(ref reference4));
                if(vector2 != Vector128<T>.Zero)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool Contains<T>(ref T searchSpace, T value, int length) where T : IEquatable<T>
    {
        nint num = 0;
        if(default(T) != null || value != null)
        {
            while(length >= 8)
            {
                length -= 8;
                if(!value.Equals(Unsafe.Add(ref searchSpace, num + 0))
                    && !value.Equals(Unsafe.Add(ref searchSpace, num + 1))
                    && !value.Equals(Unsafe.Add(ref searchSpace, num + 2))
                    && !value.Equals(Unsafe.Add(ref searchSpace, num + 3))
                    && !value.Equals(Unsafe.Add(ref searchSpace, num + 4))
                    && !value.Equals(Unsafe.Add(ref searchSpace, num + 5))
                    && !value.Equals(Unsafe.Add(ref searchSpace, num + 6))
                    && !value.Equals(Unsafe.Add(ref searchSpace, num + 7)))
                {
                    num += 8;
                    continue;
                }

                goto IL_0220;
            }

            if(length >= 4)
            {
                length -= 4;
                if(value.Equals(Unsafe.Add(ref searchSpace, num + 0))
                    || value.Equals(Unsafe.Add(ref searchSpace, num + 1))
                    || value.Equals(Unsafe.Add(ref searchSpace, num + 2))
                    || value.Equals(Unsafe.Add(ref searchSpace, num + 3)))
                {
                    goto IL_0220;
                }

                num += 4;
            }

            while(length > 0)
            {
                length--;
                if(!value.Equals(Unsafe.Add(ref searchSpace, num)))
                {
                    num++;
                    continue;
                }

                goto IL_0220;
            }
        }
        else
        {
            nint num2 = length;
            num = 0;
            while(num < num2)
            {
                if(Unsafe.Add(ref searchSpace, num) != null)
                {
                    num++;
                    continue;
                }

                goto IL_0220;
            }
        }

        return false;
    IL_0220:
        return true;
    }
}
