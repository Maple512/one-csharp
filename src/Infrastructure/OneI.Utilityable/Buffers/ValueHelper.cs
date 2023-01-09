namespace OneI.Buffers;

using System.Numerics;
using System.Runtime.Intrinsics;

/// <summary>
/// <para>source: https://github.com/dotnet/runtime/blob/ac2ffdf4ff87e7e3a5506a8ef69ce633f6fcda85/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.T.cs#L300</para>
/// </summary>
internal static unsafe partial class ValueHelper
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int IndexOfValueTypeaaa<TValue, TNegator>(ref TValue searchSpace, TValue value, int length)
        where TValue : unmanaged, IEqualityOperators<TValue, TValue, bool>
       where TNegator : struct, INegator<TValue>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            nuint num = 0u;
            while(length >= 8)
            {
                length -= 8;
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num) == value))
                {
                    return (int)num;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 1) == value))
                {
                    return (int)num + 1;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 2) == value))
                {
                    return (int)num + 2;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 3) == value))
                {
                    return (int)num + 3;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 4) == value))
                {
                    return (int)num + 4;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 5) == value))
                {
                    return (int)num + 5;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 6) == value))
                {
                    return (int)num + 6;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 7) == value))
                {
                    return (int)num + 7;
                }
                num += 8;
            }
            if(length >= 4)
            {
                length -= 4;
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num) == value))
                {
                    return (int)num;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 1) == value))
                {
                    return (int)num + 1;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 2) == value))
                {
                    return (int)num + 2;
                }
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num + 3) == value))
                {
                    return (int)num + 3;
                }
                num += 4;
            }
            while(length > 0)
            {
                length--;
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num) == value))
                {
                    return (int)num;
                }
                num++;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            Vector256<TValue> left = Vector256.Create(value);
            ref TValue reference = ref searchSpace;
            ref TValue reference2 = ref Unsafe.Add(ref searchSpace, length - Vector256<TValue>.Count);
            do
            {
                Vector256<TValue> vector = TNegator.NegateIfNeeded(Vector256.Equals(left, Vector256.LoadUnsafe(ref reference)));
                if(vector == Vector256<TValue>.Zero)
                {
                    reference = ref Unsafe.Add(ref reference, Vector256<TValue>.Count);
                    continue;
                }
                return ComputeFirstIndex(ref searchSpace, ref reference, vector);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference, ref reference2));
            if((long)(uint)length % (long)Vector256<TValue>.Count != 0L)
            {
                Vector256<TValue> vector = TNegator.NegateIfNeeded(Vector256.Equals(left, Vector256.LoadUnsafe(ref reference2)));
                if(vector != Vector256<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference2, vector);
                }
            }
        }
        else
        {
            Vector128<TValue> left2 = Vector128.Create(value);
            ref TValue reference3 = ref searchSpace;
            ref TValue reference4 = ref Unsafe.Add(ref searchSpace, length - Vector128<TValue>.Count);
            do
            {
                Vector128<TValue> vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, Vector128.LoadUnsafe(ref reference3)));
                if(vector2 == Vector128<TValue>.Zero)
                {
                    reference3 = ref Unsafe.Add(ref reference3, Vector128<TValue>.Count);
                    continue;
                }
                return ComputeFirstIndex(ref searchSpace, ref reference3, vector2);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference3, ref reference4));
            if((long)(uint)length % (long)Vector128<TValue>.Count != 0L)
            {
                Vector128<TValue> vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, Vector128.LoadUnsafe(ref reference4)));
                if(vector2 != Vector128<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference4, vector2);
                }
            }
        }
        return -1;
    }


    public static int IndexOaaaf<T>(ref T searchSpace, T value, int length)
         where T : unmanaged, IEqualityOperators<T, T, bool>
    {
        nint num = 0;
        if(default(T) != null || value != null)
        {
            while(length >= 8)
            {
                length -= 8;
                if(value.Equals(Unsafe.Add(ref searchSpace, num)))
                {
                    goto IL_021a;
                }
                if(value.Equals(Unsafe.Add(ref searchSpace, num + 1)))
                {
                    goto IL_021d;
                }
                if(value.Equals(Unsafe.Add(ref searchSpace, num + 2)))
                {
                    goto IL_0223;
                }
                if(!value.Equals(Unsafe.Add(ref searchSpace, num + 3)))
                {
                    if(!value.Equals(Unsafe.Add(ref searchSpace, num + 4)))
                    {
                        if(!value.Equals(Unsafe.Add(ref searchSpace, num + 5)))
                        {
                            if(!value.Equals(Unsafe.Add(ref searchSpace, num + 6)))
                            {
                                if(!value.Equals(Unsafe.Add(ref searchSpace, num + 7)))
                                {
                                    num += 8;
                                    continue;
                                }
                                return (int)(num + 7);
                            }
                            return (int)(num + 6);
                        }
                        return (int)(num + 5);
                    }
                    return (int)(num + 4);
                }
                goto IL_0229;
            }
            if(length >= 4)
            {
                length -= 4;
                if(value.Equals(Unsafe.Add(ref searchSpace, num)))
                {
                    goto IL_021a;
                }
                if(value.Equals(Unsafe.Add(ref searchSpace, num + 1)))
                {
                    goto IL_021d;
                }
                if(value.Equals(Unsafe.Add(ref searchSpace, num + 2)))
                {
                    goto IL_0223;
                }
                if(value.Equals(Unsafe.Add(ref searchSpace, num + 3)))
                {
                    goto IL_0229;
                }
                num += 4;
            }
            while(length > 0)
            {
                if(!value.Equals(Unsafe.Add(ref searchSpace, num)))
                {
                    num++;
                    length--;
                    continue;
                }
                goto IL_021a;
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
                goto IL_021a;
            }
        }
        return -1;
    IL_0223:
        return (int)(num + 2);
    IL_021d:
        return (int)(num + 1);
    IL_021a:
        return (int)num;
    IL_0229:
        return (int)(num + 3);
    }


    public static nuint CommonPrefixLength(ref byte first, ref byte second, nuint length)
    {
        nuint num;
        if(!Vector128.IsHardwareAccelerated || length < (nuint)Vector128<byte>.Count)
        {
            num = length % 4u;
            if(num != 0)
            {
                if(first != second)
                {
                    return 0u;
                }

                if(num > 1)
                {
                    if(Unsafe.Add(ref first, 1) != Unsafe.Add(ref second, 1))
                    {
                        return 1u;
                    }

                    if(num > 2 && Unsafe.Add(ref first, 2) != Unsafe.Add(ref second, 2))
                    {
                        return 2u;
                    }
                }
            }

            for(; (nint)num <= (nint)length - 4; num += 4)
            {
                if(Unsafe.Add(ref first, num + 0) != Unsafe.Add(ref second, num + 0))
                {
                    return num + 0;
                }

                if(Unsafe.Add(ref first, num + 1) != Unsafe.Add(ref second, num + 1))
                {
                    return num + 1;
                }

                if(Unsafe.Add(ref first, num + 2) != Unsafe.Add(ref second, num + 2))
                {
                    return num + 2;
                }

                if(Unsafe.Add(ref first, num + 3) != Unsafe.Add(ref second, num + 3))
                {
                    return num + 3;
                }
            }

            return length;
        }

        var num2 = (nuint)((nint)length - Vector128<byte>.Count);
        num = 0u;
        uint num3;
        while(true)
        {
            Vector128<byte> vector;
            if(num < num2)
            {
                vector = Vector128.Equals(Vector128.LoadUnsafe(ref first, num), Vector128.LoadUnsafe(ref second, num));
                num3 = vector.ExtractMostSignificantBits();
                if(num3 != 65535)
                {
                    break;
                }

                num += (nuint)Vector128<byte>.Count;
                continue;
            }

            num = num2;
            vector = Vector128.Equals(Vector128.LoadUnsafe(ref first, num), Vector128.LoadUnsafe(ref second, num));
            num3 = vector.ExtractMostSignificantBits();
            if(num3 != 65535)
            {
                break;
            }

            return length;
        }

        num3 = ~num3;
        return num + uint.TrailingZeroCount(num3);
    }

    public static void Fill<T>(ref T refData, nuint numElements, T value)
        where T : unmanaged
    {
        if(!RuntimeHelpers.IsReferenceOrContainsReferences<T>()
            && Vector.IsHardwareAccelerated
            && Unsafe.SizeOf<T>() <= Vector<byte>.Count
            && BitOperations.IsPow2(Unsafe.SizeOf<T>())
            && numElements >= (uint)(Vector<byte>.Count / Unsafe.SizeOf<T>()))
        {
            var source = value;
            Vector<byte> value2;
            if(Unsafe.SizeOf<T>() == 1)
            {
                value2 = new Vector<byte>(Unsafe.As<T, byte>(ref source));
            }
            else if(Unsafe.SizeOf<T>() == 2)
            {
                value2 = (Vector<byte>)new Vector<ushort>(Unsafe.As<T, ushort>(ref source));
            }
            else if(Unsafe.SizeOf<T>() == 4)
            {
                value2 = (typeof(T) == typeof(float))
                    ? ((Vector<byte>)new Vector<float>((float)(object)source))
                    : ((Vector<byte>)new Vector<uint>(Unsafe.As<T, uint>(ref source)));
            }
            else if(Unsafe.SizeOf<T>() == 8)
            {
                value2 = (typeof(T) == typeof(double))
                    ? ((Vector<byte>)new Vector<double>((double)(object)source))
                    : ((Vector<byte>)new Vector<ulong>(Unsafe.As<T, ulong>(ref source)));
            }
            else if(Unsafe.SizeOf<T>() == 16)
            {
                var vector = Unsafe.As<T, Vector128<byte>>(ref source);
                if(Vector<byte>.Count == 16)
                {
                    value2 = vector.AsVector();
                }
                else
                {
                    if(Vector<byte>.Count != 32)
                    {
                        goto IL_022d;
                    }

                    value2 = Vector256.Create(vector, vector).AsVector();
                }
            }
            else
            {
                if(Unsafe.SizeOf<T>() != 32 || Vector<byte>.Count != 32)
                {
                    goto IL_022d;
                }

                value2 = Unsafe.As<T, Vector256<byte>>(ref source).AsVector();
            }

            ref var source2 = ref Unsafe.As<T, byte>(ref refData);
            var num = (nuint)((nint)numElements * Unsafe.SizeOf<T>());
            var num2 = num & (nuint)(2 * -Vector<byte>.Count);
            nuint num3 = 0u;
            if(numElements >= (uint)(2 * Vector<byte>.Count / Unsafe.SizeOf<T>()))
            {
                do
                {
                    Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref source2, num3), value2);
                    Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref source2, num3 + (nuint)Vector<byte>.Count), value2);
                    num3 += (uint)(2 * Vector<byte>.Count);
                }
                while(num3 < num2);
            }

            if((num & (nuint)Vector<byte>.Count) != 0)
            {
                Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref source2, num3), value2);
            }

            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref source2, num - (nuint)Vector<byte>.Count), value2);
            return;
        }

        goto IL_022d;
    IL_022d:
        nuint num4 = 0u;
        if(numElements >= 8)
        {
            var num5 = (nuint)(nint)numElements & ~(nuint)7u;
            do
            {
                Unsafe.Add(ref refData, (nint)(num4 + 0)) = value;
                Unsafe.Add(ref refData, (nint)(num4 + 1)) = value;
                Unsafe.Add(ref refData, (nint)(num4 + 2)) = value;
                Unsafe.Add(ref refData, (nint)(num4 + 3)) = value;
                Unsafe.Add(ref refData, (nint)(num4 + 4)) = value;
                Unsafe.Add(ref refData, (nint)(num4 + 5)) = value;
                Unsafe.Add(ref refData, (nint)(num4 + 6)) = value;
                Unsafe.Add(ref refData, (nint)(num4 + 7)) = value;
            }
            while((num4 += 8) < num5);
        }

        if(((nint)numElements & 4) != 0)
        {
            Unsafe.Add(ref refData, (nint)(num4 + 0)) = value;
            Unsafe.Add(ref refData, (nint)(num4 + 1)) = value;
            Unsafe.Add(ref refData, (nint)(num4 + 2)) = value;
            Unsafe.Add(ref refData, (nint)(num4 + 3)) = value;
            num4 += 4;
        }

        if(((nint)numElements & 2) != 0)
        {
            Unsafe.Add(ref refData, (nint)(num4 + 0)) = value;
            Unsafe.Add(ref refData, (nint)(num4 + 1)) = value;
            num4 += 2;
        }

        if(((nint)numElements & 1) != 0)
        {
            Unsafe.Add(ref refData, (nint)num4) = value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static unsafe int IndexOfNullCharacter(ref char searchSpace)
    {
        nint num = 0;
        nint num2 = int.MaxValue;
        if(((int)Unsafe.AsPointer(ref searchSpace) & 1) == 0)
        {
            if(Vector128.IsHardwareAccelerated)
            {
                num2 = UnalignedCountVector128(ref searchSpace);
            }
            else if(Vector.IsHardwareAccelerated)
            {
                num2 = UnalignedCountVector(ref searchSpace);
            }
        }

        while(true)
        {
            if(num2 >= 4)
            {
                ref var reference = ref Unsafe.Add(ref searchSpace, num);
                if(reference == '\0')
                {
                    break;
                }

                if(Unsafe.Add(ref reference, 1) == '\0')
                {
                    return (int)(num + 1);
                }

                if(Unsafe.Add(ref reference, 2) == '\0')
                {
                    return (int)(num + 2);
                }

                if(Unsafe.Add(ref reference, 3) != 0)
                {
                    num += 4;
                    num2 -= 4;
                    continue;
                }
            }
            else
            {
                while(num2 > 0)
                {
                    if(Unsafe.Add(ref searchSpace, num) == '\0')
                    {
                        goto end_IL_0075;
                    }

                    num++;
                    num2--;
                }

                if(Vector256.IsHardwareAccelerated)
                {
                    if(num < int.MaxValue)
                    {
                        ref var source = ref Unsafe.As<char, ushort>(ref searchSpace);
                        if(((nuint)Unsafe.AsPointer(ref Unsafe.Add(ref searchSpace, num)) & (nuint)(Vector256<byte>.Count - 1)) != 0)
                        {
                            var right = Vector128.LoadUnsafe(ref source, (nuint)num);
                            var num3 = Vector128.Equals(Vector128<ushort>.Zero, right).AsByte().ExtractMostSignificantBits();
                            if(num3 != 0)
                            {
                                return (int)(num + (uint)BitOperations.TrailingZeroCount(num3) / 2u);
                            }

                            num += Vector128<ushort>.Count;
                        }

                        num2 = GetCharVector256SpanLength(num, int.MaxValue);
                        if(num2 > 0)
                        {
                            do
                            {
                                var right2 = Vector256.LoadUnsafe(ref source, (nuint)num);
                                var num4 = Vector256.Equals(Vector256<ushort>.Zero, right2).AsByte().ExtractMostSignificantBits();
                                if(num4 == 0)
                                {
                                    num += Vector256<ushort>.Count;
                                    num2 -= Vector256<ushort>.Count;
                                    continue;
                                }

                                return (int)(num + (uint)BitOperations.TrailingZeroCount(num4) / 2u);
                            }
                            while(num2 > 0);
                        }

                        num2 = GetCharVector128SpanLength(num, int.MaxValue);
                        if(num2 > 0)
                        {
                            var right3 = Vector128.LoadUnsafe(ref source, (nuint)num);
                            var num5 = Vector128.Equals(Vector128<ushort>.Zero, right3).AsByte().ExtractMostSignificantBits();
                            if(num5 != 0)
                            {
                                return (int)(num + (uint)BitOperations.TrailingZeroCount(num5) / 2u);
                            }

                            num += Vector128<ushort>.Count;
                        }

                        if(num < int.MaxValue)
                        {
                            num2 = int.MaxValue - num;
                            continue;
                        }
                    }
                }
                else if(Vector128.IsHardwareAccelerated)
                {
                    if(num < int.MaxValue)
                    {
                        ref var source2 = ref Unsafe.As<char, ushort>(ref searchSpace);
                        num2 = GetCharVector128SpanLength(num, int.MaxValue);
                        if(num2 > 0)
                        {
                            do
                            {
                                var right4 = Vector128.LoadUnsafe(ref source2, (uint)num);
                                var vector = Vector128.Equals(Vector128<ushort>.Zero, right4);
                                if(vector == Vector128<ushort>.Zero)
                                {
                                    num += Vector128<ushort>.Count;
                                    num2 -= Vector128<ushort>.Count;
                                    continue;
                                }

                                var value = vector.AsByte().ExtractMostSignificantBits();
                                return (int)(num + (uint)BitOperations.TrailingZeroCount(value) / 2u);
                            }
                            while(num2 > 0);
                        }

                        if(num < int.MaxValue)
                        {
                            num2 = int.MaxValue - num;
                            continue;
                        }
                    }
                }
                else if(Vector.IsHardwareAccelerated && num < int.MaxValue)
                {
                    num2 = GetCharVectorSpanLength(num, int.MaxValue);
                    if(num2 > 0)
                    {
                        do
                        {
                            var vector2 = Vector.Equals(Vector<ushort>.Zero, LoadVector(ref searchSpace, num));
                            if(Vector<ushort>.Zero.Equals(vector2))
                            {
                                num += Vector<ushort>.Count;
                                num2 -= Vector<ushort>.Count;
                                continue;
                            }

                            return (int)(num + LocateFirstFoundChar(vector2));
                        }
                        while(num2 > 0);
                    }

                    if(num < int.MaxValue)
                    {
                        num2 = int.MaxValue - num;
                        continue;
                    }
                }

                ThrowMustBeNullTerminatedString();
            }

            return (int)(num + 3);
        end_IL_0075:
            break;
        }

        return (int)num;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfChar(ref char searchSpace, char value, int length)
    {
        return IndexOfValueType(ref Unsafe.As<char, short>(ref searchSpace), (short)value, length);
    }

    /// <summary>
    /// Reset specific bit in the given value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ResetBit(uint value, int bitPos)
    {
        // TODO: Recognize BTR on x86 and LSL+BIC on ARM
        return value & ~(uint)(1 << bitPos);
    }

    /// <summary>
    /// Reset the lowest significant bit in the given value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ResetLowestSetBit(uint value)
    {
        // It's lowered to BLSR on x86
        return value & (value - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nint GetCharVector128SpanLength(nint offset, nint length)
    {
        return (length - offset) & ~(Vector128<ushort>.Count - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nint GetCharVector256SpanLength(nint offset, nint length)
    {
        return (length - offset) & ~(Vector256<ushort>.Count - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nint GetCharVectorSpanLength(nint offset, nint length)
    {
        return (length - offset) & ~(Vector<ushort>.Count - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector<byte> LoadVector(ref byte start, nuint offset)
    {
        return Unsafe.ReadUnaligned<Vector<byte>>(ref Unsafe.AddByteOffset(ref start, offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector<ushort> LoadVector(ref char start, nint offset)
    {
        return Unsafe.ReadUnaligned<Vector<ushort>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref start, offset)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LocateFirstFoundChar(Vector<ushort> match)
    {
        var vector = Vector.AsVectorUInt64(match);
        var num = 0uL;
        int i;
        for(i = 0; i < Vector<ulong>.Count; i++)
        {
            num = vector[i];
            if(num != 0L)
            {
                break;
            }
        }

        return i * 4 + LocateFirstFoundChar(num);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LocateFirstFoundChar(ulong match)
    {
        return BitOperations.TrailingZeroCount(match) >> 4;
    }

    [DoesNotReturn]
    private static void ThrowMustBeNullTerminatedString()
    {
        throw new ArgumentException("The string must be null-terminated.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe nint UnalignedCountVector(ref char searchSpace)
    {
        return (nint)(uint)(-(int)Unsafe.AsPointer(ref searchSpace) / 2) & Vector<ushort>.Count - 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe nuint UnalignedCountVector128(ref byte searchSpace)
    {
        var num = (nint)Unsafe.AsPointer(ref searchSpace) & Vector128<byte>.Count - 1;
        return (uint)((Vector128<byte>.Count - num) & (Vector128<byte>.Count - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe nint UnalignedCountVector128(ref char searchSpace)
    {
        return (nint)(uint)(-(int)Unsafe.AsPointer(ref searchSpace) / 2) & Vector128<ushort>.Count - 1;
    }

    public static void Replace<T>(ValueBuffer<T> span, T oldValue, T newValue)
        where T : unmanaged, IEquatable<T>?
    {
        for(var i = 0; i < span.Length; ++i)
        {
            ref var val = ref span[i];

            if(oldValue.Equals(val))
            {
                val = newValue;
            }
        }
    }

    public static void ReplaceValueType<T>(ref T src, ref T dst, T oldValue, T newValue, nuint length)
        where T : unmanaged
    {
        if(!Vector128.IsHardwareAccelerated || length < (uint)Vector128<T>.Count)
        {
            for(nuint idx = 0; idx < length; ++idx)
            {
                var original = Unsafe.Add(ref src, idx);
                Unsafe.Add(ref dst, idx) = EqualityComparer<T>.Default.Equals(original, oldValue) ? newValue : original;
            }
        }
        else
        {
            Debug.Assert(Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported, "Vector128 is not HW-accelerated or not supported");

            nuint idx = 0;

            if(!Vector256.IsHardwareAccelerated || length < (uint)Vector256<T>.Count)
            {
                var lastVectorIndex = length - (uint)Vector128<T>.Count;
                var oldValues = Vector128.Create(oldValue);
                var newValues = Vector128.Create(newValue);
                Vector128<T> original, mask, result;

                do
                {
                    original = Vector128.LoadUnsafe(ref src, idx);
                    mask = Vector128.Equals(oldValues, original);
                    result = Vector128.ConditionalSelect(mask, newValues, original);
                    result.StoreUnsafe(ref dst, idx);

                    idx += (uint)Vector128<T>.Count;
                }
                while(idx < lastVectorIndex);

                // There are (0, Vector128<T>.Count] elements remaining now.
                // As the operation is idempotent, and we know that in total there are at least Vector128<T>.Count
                // elements available, we read a vector from the very end, perform the replace and write to the
                // the resulting vector at the very end.
                // Thus we can eliminate the scalar processing of the remaining elements.
                original = Vector128.LoadUnsafe(ref src, lastVectorIndex);
                mask = Vector128.Equals(oldValues, original);
                result = Vector128.ConditionalSelect(mask, newValues, original);
                result.StoreUnsafe(ref dst, lastVectorIndex);
            }
            else
            {
                Debug.Assert(Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported, "Vector256 is not HW-accelerated or not supported");

                var lastVectorIndex = length - (uint)Vector256<T>.Count;
                var oldValues = Vector256.Create(oldValue);
                var newValues = Vector256.Create(newValue);
                Vector256<T> original, mask, result;

                do
                {
                    original = Vector256.LoadUnsafe(ref src, idx);
                    mask = Vector256.Equals(oldValues, original);
                    result = Vector256.ConditionalSelect(mask, newValues, original);
                    result.StoreUnsafe(ref dst, idx);

                    idx += (uint)Vector256<T>.Count;
                }
                while(idx < lastVectorIndex);

                original = Vector256.LoadUnsafe(ref src, lastVectorIndex);
                mask = Vector256.Equals(oldValues, original);
                result = Vector256.ConditionalSelect(mask, newValues, original);
                result.StoreUnsafe(ref dst, lastVectorIndex);
            }
        }
    }

    internal readonly struct ComparerComparable<T, TComparer> : IComparable<T> where TComparer : IComparer<T>
    {
        private readonly TComparer _comparer;
        private readonly T _value;
        public ComparerComparable(T value, TComparer comparer)
        {
            _value = value;
            _comparer = comparer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(T? other)
        {
            var comparer = _comparer;

            return comparer.Compare(_value, other);
        }
    }

    internal interface INegator<T> where T : struct
    {
        static abstract bool NegateIfNeeded(bool equals);

        static abstract Vector128<T> NegateIfNeeded(Vector128<T> equals);

        static abstract Vector256<T> NegateIfNeeded(Vector256<T> equals);
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal readonly struct DontNegate<T> : INegator<T> where T : struct
    {
        public static bool NegateIfNeeded(bool equals)
        {
            return equals;
        }

        public static Vector128<T> NegateIfNeeded(Vector128<T> equals)
        {
            return equals;
        }

        public static Vector256<T> NegateIfNeeded(Vector256<T> equals)
        {
            return equals;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal readonly struct Negate<T> : INegator<T> where T : struct
    {
        public static bool NegateIfNeeded(bool equals)
        {
            return !equals;
        }

        public static Vector128<T> NegateIfNeeded(Vector128<T> equals)
        {
            return ~equals;
        }

        public static Vector256<T> NegateIfNeeded(Vector256<T> equals)
        {
            return ~equals;
        }
    }
}
