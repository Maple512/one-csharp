namespace OneI.Buffers;

using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

internal static partial class ValueHelper
{
    public static void Reverse(ref byte buf, nuint length)
    {
        if(Avx2.IsSupported && (nuint)(Vector256<byte>.Count * (nint)2) <= length)
        {
            var mask = Vector256.Create(15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, (byte)0);
            var num = (nuint)Vector256<byte>.Count;
            var num2 = length / num / 2u;
            for(nuint num3 = 0u; num3 < num2; num3++)
            {
                var elementOffset = num3 * num;
                var elementOffset2 = (nuint)(nint)length - (1 + num3) * num;
                var value = Vector256.LoadUnsafe(ref buf, elementOffset);
                var value2 = Vector256.LoadUnsafe(ref buf, elementOffset2);
                value = Avx2.Shuffle(value, mask);
                value = Avx2.Permute2x128(value, value, 1);
                value2 = Avx2.Shuffle(value2, mask);
                value2 = Avx2.Permute2x128(value2, value2, 1);
                value2.StoreUnsafe(ref buf, elementOffset);
                value.StoreUnsafe(ref buf, elementOffset2);
            }

            buf = ref Unsafe.Add(ref buf, num2 * num);
            length = (nuint)(nint)length - num2 * num * 2;
        }
        else if(Vector128.IsHardwareAccelerated && (nuint)(Vector128<byte>.Count * (nint)2) <= length)
        {
            var num4 = (nuint)Vector128<byte>.Count;
            var num5 = length / num4 / 2u;
            for(nuint num6 = 0u; num6 < num5; num6++)
            {
                var elementOffset3 = num6 * num4;
                var elementOffset4 = (nuint)(nint)length - (1 + num6) * num4;
                var vector = Vector128.LoadUnsafe(ref buf, elementOffset3);
                var vector2 = Vector128.LoadUnsafe(ref buf, elementOffset4);
                vector = Vector128.Shuffle(vector, Vector128.Create(15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, (byte)0));
                vector2 = Vector128.Shuffle(vector2, Vector128.Create(15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, (byte)0));
                vector2.StoreUnsafe(ref buf, elementOffset3);
                vector.StoreUnsafe(ref buf, elementOffset4);
            }

            buf = ref Unsafe.Add(ref buf, num5 * num4);
            length = (nuint)(nint)length - num5 * num4 * 2;
        }

        ReverseInner(ref buf, length);
    }

    public static void Reverse(ref char buf, nuint length)
    {
        if(Avx2.IsSupported && (nuint)(Vector256<short>.Count * (nint)2) <= length)
        {
            ref var reference = ref Unsafe.As<char, byte>(ref buf);
            var num = (nuint)((nint)length * 2);
            var mask = Vector256.Create(14, 15, 12, 13, 10, 11, 8, 9, 6, 7, 4, 5, 2, 3, 0, 1, 14, 15, 12, 13, 10, 11, 8, 9, 6, 7, 4, 5, 2, 3, 0, (byte)1);
            var num2 = (nuint)Vector256<byte>.Count;
            var num3 = num / num2 / 2u;
            for(nuint num4 = 0u; num4 < num3; num4++)
            {
                var elementOffset = num4 * num2;
                var elementOffset2 = num - (1 + num4) * num2;
                var value = Vector256.LoadUnsafe(ref reference, elementOffset);
                var value2 = Vector256.LoadUnsafe(ref reference, elementOffset2);
                value = Avx2.Shuffle(value, mask);
                value = Avx2.Permute2x128(value, value, 1);
                value2 = Avx2.Shuffle(value2, mask);
                value2 = Avx2.Permute2x128(value2, value2, 1);
                value2.StoreUnsafe(ref reference, elementOffset);
                value.StoreUnsafe(ref reference, elementOffset2);
            }

            reference = ref Unsafe.Add(ref reference, num3 * num2);
            length = (nuint)((nint)length - (nint)num3 * Vector256<short>.Count * 2);
            buf = ref Unsafe.As<byte, char>(ref reference);
        }
        else if(Vector128.IsHardwareAccelerated && (nuint)(Vector128<short>.Count * (nint)2) <= length)
        {
            ref var reference2 = ref Unsafe.As<char, short>(ref buf);
            var num5 = (nuint)Vector128<short>.Count;
            var num6 = length / num5 / 2u;
            for(nuint num7 = 0u; num7 < num6; num7++)
            {
                var elementOffset3 = num7 * num5;
                var elementOffset4 = (nuint)(nint)length - (1 + num7) * num5;
                var vector = Vector128.LoadUnsafe(ref reference2, elementOffset3);
                var vector2 = Vector128.LoadUnsafe(ref reference2, elementOffset4);
                vector = Vector128.Shuffle(vector, Vector128.Create(7, 6, 5, 4, 3, 2, 1, 0));
                vector2 = Vector128.Shuffle(vector2, Vector128.Create(7, 6, 5, 4, 3, 2, 1, 0));
                vector2.StoreUnsafe(ref reference2, elementOffset3);
                vector.StoreUnsafe(ref reference2, elementOffset4);
            }

            reference2 = ref Unsafe.Add(ref reference2, num6 * num5);
            length = (nuint)((nint)length - (nint)num6 * Vector128<short>.Count * 2);
            buf = ref Unsafe.As<short, char>(ref reference2);
        }

        ReverseInner(ref buf, length);
    }

    public static void Reverse(ref int buf, nuint length)
    {
        if(Avx2.IsSupported && (nuint)(Vector256<int>.Count * (nint)2) <= length)
        {
            var num = (nuint)Vector256<int>.Count;
            var num2 = length / num / 2u;
            var control = Vector256.Create(7, 6, 5, 4, 3, 2, 1, 0);
            for(nuint num3 = 0u; num3 < num2; num3++)
            {
                var elementOffset = num3 * num;
                var elementOffset2 = (nuint)(nint)length - (1 + num3) * num;
                var left = Vector256.LoadUnsafe(ref buf, elementOffset);
                var left2 = Vector256.LoadUnsafe(ref buf, elementOffset2);
                left = Avx2.PermuteVar8x32(left, control);
                left2 = Avx2.PermuteVar8x32(left2, control);
                left2.StoreUnsafe(ref buf, elementOffset);
                left.StoreUnsafe(ref buf, elementOffset2);
            }

            buf = ref Unsafe.Add(ref buf, num2 * num);
            length = (nuint)(nint)length - num2 * num * 2;
        }
        else if(Vector128.IsHardwareAccelerated && (nuint)(Vector128<int>.Count * (nint)2) <= length)
        {
            var num4 = (nuint)Vector128<int>.Count;
            var num5 = length / num4 / 2u;
            for(nuint num6 = 0u; num6 < num5; num6++)
            {
                var elementOffset3 = num6 * num4;
                var elementOffset4 = (nuint)(nint)length - (1 + num6) * num4;
                var vector = Vector128.LoadUnsafe(ref buf, elementOffset3);
                var vector2 = Vector128.LoadUnsafe(ref buf, elementOffset4);
                vector = Vector128.Shuffle(vector, Vector128.Create(3, 2, 1, 0));
                vector2 = Vector128.Shuffle(vector2, Vector128.Create(3, 2, 1, 0));
                vector2.StoreUnsafe(ref buf, elementOffset3);
                vector.StoreUnsafe(ref buf, elementOffset4);
            }

            buf = ref Unsafe.Add(ref buf, num5 * num4);
            length = (nuint)(nint)length - num5 * num4 * 2;
        }

        ReverseInner(ref buf, length);
    }

    public static void Reverse(ref long buf, nuint length)
    {
        if(Avx2.IsSupported && (nuint)(Vector256<long>.Count * (nint)2) <= length)
        {
            var num = (nuint)Vector256<long>.Count;
            var num2 = length / num / 2u;
            for(nuint num3 = 0u; num3 < num2; num3++)
            {
                var elementOffset = num3 * num;
                var elementOffset2 = (nuint)(nint)length - (1 + num3) * num;
                var value = Vector256.LoadUnsafe(ref buf, elementOffset);
                var value2 = Vector256.LoadUnsafe(ref buf, elementOffset2);
                value = Avx2.Permute4x64(value, 27);
                value2 = Avx2.Permute4x64(value2, 27);
                value2.StoreUnsafe(ref buf, elementOffset);
                value.StoreUnsafe(ref buf, elementOffset2);
            }

            buf = ref Unsafe.Add(ref buf, num2 * num);
            length = (nuint)(nint)length - num2 * num * 2;
        }
        else if(Vector128.IsHardwareAccelerated && (nuint)(Vector128<long>.Count * (nint)2) <= length)
        {
            var num4 = (nuint)Vector128<long>.Count;
            var num5 = length / num4 / 2u;
            for(nuint num6 = 0u; num6 < num5; num6++)
            {
                var elementOffset3 = num6 * num4;
                var elementOffset4 = (nuint)(nint)length - (1 + num6) * num4;
                var vector = Vector128.LoadUnsafe(ref buf, elementOffset3);
                var vector2 = Vector128.LoadUnsafe(ref buf, elementOffset4);
                vector = Vector128.Shuffle(vector, Vector128.Create(1L, 0L));
                vector2 = Vector128.Shuffle(vector2, Vector128.Create(1L, 0L));
                vector2.StoreUnsafe(ref buf, elementOffset3);
                vector.StoreUnsafe(ref buf, elementOffset4);
            }

            buf = ref Unsafe.Add(ref buf, num5 * num4);
            length = (nuint)((nint)length - (nint)num5 * Vector128<long>.Count * 2);
        }

        ReverseInner(ref buf, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Reverse<T>(ref T elements, nuint length)
        where T : unmanaged
    {
        if(!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            if(Unsafe.SizeOf<T>() == 1)
            {
                Reverse(ref Unsafe.As<T, byte>(ref elements), length);
                return;
            }

            if(Unsafe.SizeOf<T>() == 2)
            {
                Reverse(ref Unsafe.As<T, char>(ref elements), length);
                return;
            }

            if(Unsafe.SizeOf<T>() == 4)
            {
                Reverse(ref Unsafe.As<T, int>(ref elements), length);
                return;
            }

            if(Unsafe.SizeOf<T>() == 8)
            {
                Reverse(ref Unsafe.As<T, long>(ref elements), length);
                return;
            }
        }

        ReverseInner(ref elements, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReverseInner<T>(ref T elements, nuint length)
        where T : unmanaged
    {
        if(length > 1)
        {
            ref var reference = ref elements;
            ref var reference2 = ref Unsafe.Subtract(ref Unsafe.Add(ref reference, (int)length), 1);
            do
            {
                (reference2, reference) = (reference, reference2);
                reference = ref Unsafe.Add(ref reference, 1);
                reference2 = ref Unsafe.Subtract(ref reference2, 1);
            }
            while(Unsafe.IsAddressLessThan(ref reference, ref reference2));
        }
    }
}
