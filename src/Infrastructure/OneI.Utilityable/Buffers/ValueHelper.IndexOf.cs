namespace OneI.Buffers;

using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

internal static partial class ValueHelper
{
    public static int IndexOf(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
    {
        if(valueLength == 0)
        {
            return 0;
        }

        var num = valueLength - 1;
        if(num == 0)
        {
            return IndexOfValueType(ref searchSpace, value, searchSpaceLength);
        }

        nint num2 = 0;
        var value2 = value;
        var num3 = searchSpaceLength - num;
        if(!Vector128.IsHardwareAccelerated || num3 < Vector128<byte>.Count)
        {
            ref var second = ref Unsafe.Add(ref value, 1);
            var num4 = num3;
            while(num4 > 0)
            {
                var num5 = IndexOfValueType(ref Unsafe.Add(ref searchSpace, num2), value2, num4);
                if(num5 < 0)
                {
                    break;
                }

                num4 -= num5;
                num2 += num5;
                if(num4 <= 0)
                {
                    break;
                }

                if(SequenceEqual(ref Unsafe.Add(ref searchSpace, num2 + 1), ref second, (uint)num))
                {
                    return (int)num2;
                }

                num4--;
                num2++;
            }

            return -1;
        }

        if(Vector256.IsHardwareAccelerated && num3 - Vector256<byte>.Count >= 0)
        {
            var b = Unsafe.Add(ref value, num);
            nint num6 = num;
            while(b == value && num6 > 1)
            {
                b = Unsafe.Add(ref value, --num6);
            }

            var left = Vector256.Create(value);
            var left2 = Vector256.Create(b);
            var num7 = num3 - (nint)Vector256<byte>.Count;
            while(true)
            {
                var vector = Vector256.Equals(left2, Vector256.LoadUnsafe(ref searchSpace, (nuint)(num2 + num6)));
                var vector2 = Vector256.Equals(left, Vector256.LoadUnsafe(ref searchSpace, (nuint)num2));
                var vector3 = (vector2 & vector).AsByte();
                if(vector3 != Vector256<byte>.Zero)
                {
                    var num8 = vector3.ExtractMostSignificantBits();
                    do
                    {
                        var num9 = BitOperations.TrailingZeroCount(num8);
                        if(valueLength == 2 || SequenceEqual(ref Unsafe.Add(ref searchSpace, num2 + num9), ref value, (uint)valueLength))
                        {
                            return (int)(num2 + num9);
                        }

                        num8 = ResetLowestSetBit(num8);
                    }
                    while(num8 != 0);
                }

                num2 += Vector256<byte>.Count;
                if(num2 == num3)
                {
                    break;
                }

                if(num2 > num7)
                {
                    num2 = num7;
                }
            }

            return -1;
        }

        var b2 = Unsafe.Add(ref value, num);
        var num10 = num;
        while(b2 == value && num10 > 1)
        {
            b2 = Unsafe.Add(ref value, --num10);
        }

        var left3 = Vector128.Create(value);
        var left4 = Vector128.Create(b2);
        var num11 = num3 - (nint)Vector128<byte>.Count;
        while(true)
        {
            var vector4 = Vector128.Equals(left4, Vector128.LoadUnsafe(ref searchSpace, (nuint)(num2 + num10)));
            var vector5 = Vector128.Equals(left3, Vector128.LoadUnsafe(ref searchSpace, (nuint)num2));
            var vector6 = (vector5 & vector4).AsByte();
            if(vector6 != Vector128<byte>.Zero)
            {
                var num12 = vector6.ExtractMostSignificantBits();
                do
                {
                    var num13 = BitOperations.TrailingZeroCount(num12);
                    if(valueLength == 2 || SequenceEqual(ref Unsafe.Add(ref searchSpace, num2 + num13), ref value, (uint)valueLength))
                    {
                        return (int)(num2 + num13);
                    }

                    num12 = ResetLowestSetBit(num12);
                }
                while(num12 != 0);
            }

            num2 += Vector128<byte>.Count;
            if(num2 == num3)
            {
                break;
            }

            if(num2 > num11)
            {
                num2 = num11;
            }
        }

        return -1;
    }

    public static int IndexOf(ref char searchSpace, int searchSpaceLength, ref char value, int valueLength)
    {
        if(valueLength == 0)
        {
            return 0;
        }

        var num = valueLength - 1;
        if(num == 0)
        {
            return IndexOfChar(ref searchSpace, value, searchSpaceLength);
        }

        nint num2 = 0;
        var c = value;
        var num3 = searchSpaceLength - num;
        if(!Vector128.IsHardwareAccelerated || num3 < Vector128<ushort>.Count)
        {
            ref var second = ref Unsafe.As<char, byte>(ref Unsafe.Add(ref value, 1));
            var num4 = num3;
            while(num4 > 0)
            {
                var num5 = IndexOfChar(ref Unsafe.Add(ref searchSpace, num2), c, num4);
                if(num5 < 0)
                {
                    break;
                }

                num4 -= num5;
                num2 += num5;
                if(num4 <= 0)
                {
                    break;
                }

                if(SequenceEqual(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref searchSpace, num2 + 1)), ref second, (uint)num * (nuint)2u))
                {
                    return (int)num2;
                }

                num4--;
                num2++;
            }

            return -1;
        }

        ref var source = ref Unsafe.As<char, ushort>(ref searchSpace);
        if(Vector256.IsHardwareAccelerated && num3 - Vector256<ushort>.Count >= 0)
        {
            ushort num6 = Unsafe.Add(ref value, num);
            nint num7 = num;
            while(num6 == c && num7 > 1)
            {
                num6 = Unsafe.Add(ref value, --num7);
            }

            var left = Vector256.Create((ushort)c);
            var left2 = Vector256.Create(num6);
            var num8 = num3 - (nint)Vector256<ushort>.Count;
            while(true)
            {
                var vector = Vector256.Equals(left2, Vector256.LoadUnsafe(ref source, (nuint)(num2 + num7)));
                var vector2 = Vector256.Equals(left, Vector256.LoadUnsafe(ref source, (nuint)num2));
                var vector3 = (vector2 & vector).AsByte();
                if(vector3 != Vector256<byte>.Zero)
                {
                    var num9 = vector3.ExtractMostSignificantBits();
                    do
                    {
                        var num10 = BitOperations.TrailingZeroCount(num9);
                        var num11 = (nint)((uint)num10 / 2u);
                        if(valueLength == 2 || SequenceEqual(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref searchSpace, num2 + num11)), ref Unsafe.As<char, byte>(ref value), (uint)valueLength * (nuint)2u))
                        {
                            return (int)(num2 + num11);
                        }

                        num9 = (!Bmi1.IsSupported) ? (num9 & (uint)~(3 << num10)) : Bmi1.ResetLowestSetBit(Bmi1.ResetLowestSetBit(num9));
                    }
                    while(num9 != 0);
                }

                num2 += Vector256<ushort>.Count;
                if(num2 == num3)
                {
                    break;
                }

                if(num2 > num8)
                {
                    num2 = num8;
                }
            }

            return -1;
        }

        ushort num12 = Unsafe.Add(ref value, num);
        nint num13 = num;
        while(num12 == c && num13 > 1)
        {
            num12 = Unsafe.Add(ref value, --num13);
        }

        var left3 = Vector128.Create((ushort)c);
        var left4 = Vector128.Create(num12);
        var num14 = num3 - (nint)Vector128<ushort>.Count;
        while(true)
        {
            var vector4 = Vector128.Equals(left4, Vector128.LoadUnsafe(ref source, (nuint)(num2 + num13)));
            var vector5 = Vector128.Equals(left3, Vector128.LoadUnsafe(ref source, (nuint)num2));
            var vector6 = (vector5 & vector4).AsByte();
            if(vector6 != Vector128<byte>.Zero)
            {
                var num15 = vector6.ExtractMostSignificantBits();
                do
                {
                    var num16 = BitOperations.TrailingZeroCount(num15);
                    var num17 = (int)((uint)num16 / 2u);
                    if(valueLength == 2 || SequenceEqual(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref searchSpace, num2 + num17)), ref Unsafe.As<char, byte>(ref value), (uint)valueLength * (nuint)2u))
                    {
                        return (int)(num2 + num17);
                    }

                    num15 = (!Bmi1.IsSupported) ? (num15 & (uint)~(3 << num16)) : Bmi1.ResetLowestSetBit(Bmi1.ResetLowestSetBit(num15));
                }
                while(num15 != 0);
            }

            num2 += Vector128<ushort>.Count;
            if(num2 == num3)
            {
                break;
            }

            if(num2 > num14)
            {
                num2 = num14;
            }
        }

        return -1;
    }

    public static int IndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
    {
        if(valueLength == 0)
        {
            return 0;
        }

        var value2 = value;
        ref var second = ref Unsafe.Add(ref value, 1);
        var num = valueLength - 1;
        var num2 = 0;
        while(true)
        {
            var num3 = searchSpaceLength - num2 - num;
            if(num3 <= 0)
            {
                break;
            }

            var num4 = IndexOf(ref Unsafe.Add(ref searchSpace, num2), value2, num3);
            if(num4 < 0)
            {
                break;
            }

            num2 += num4;
            if(SequenceEqual(ref Unsafe.Add(ref searchSpace, num2 + 1), ref second, num))
            {
                return num2;
            }

            num2++;
        }

        return -1;
    }

    public static int IndexOf<T>(ref T searchSpace, T value, int length) where T : IEquatable<T>
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

    public static int IndexOfAny<T>(ref T searchSpace, T value0, T value1, int length) where T : IEquatable<T>
    {
        var i = 0;
        if(default(T) != null || value0 != null && value1 != null)
        {
            while(length - i >= 8)
            {
                var other = Unsafe.Add(ref searchSpace, i);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_0350;
                }

                other = Unsafe.Add(ref searchSpace, i + 1);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_0352;
                }

                other = Unsafe.Add(ref searchSpace, i + 2);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_0356;
                }

                other = Unsafe.Add(ref searchSpace, i + 3);
                if(!value0.Equals(other) && !value1.Equals(other))
                {
                    other = Unsafe.Add(ref searchSpace, i + 4);
                    if(!value0.Equals(other) && !value1.Equals(other))
                    {
                        other = Unsafe.Add(ref searchSpace, i + 5);
                        if(!value0.Equals(other) && !value1.Equals(other))
                        {
                            other = Unsafe.Add(ref searchSpace, i + 6);
                            if(!value0.Equals(other) && !value1.Equals(other))
                            {
                                other = Unsafe.Add(ref searchSpace, i + 7);
                                if(!value0.Equals(other) && !value1.Equals(other))
                                {
                                    i += 8;
                                    continue;
                                }

                                return i + 7;
                            }

                            return i + 6;
                        }

                        return i + 5;
                    }

                    return i + 4;
                }

                goto IL_035a;
            }

            if(length - i >= 4)
            {
                var other = Unsafe.Add(ref searchSpace, i);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_0350;
                }

                other = Unsafe.Add(ref searchSpace, i + 1);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_0352;
                }

                other = Unsafe.Add(ref searchSpace, i + 2);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_0356;
                }

                other = Unsafe.Add(ref searchSpace, i + 3);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_035a;
                }

                i += 4;
            }

            while(i < length)
            {
                var other = Unsafe.Add(ref searchSpace, i);
                if(!value0.Equals(other) && !value1.Equals(other))
                {
                    i++;
                    continue;
                }

                goto IL_0350;
            }
        }
        else
        {
            for(i = 0; i < length; i++)
            {
                var other = Unsafe.Add(ref searchSpace, i);
                if(other == null)
                {
                    if(value0 != null && value1 != null)
                    {
                        continue;
                    }
                }
                else if(!other.Equals(value0) && !other.Equals(value1))
                {
                    continue;
                }

                goto IL_0350;
            }
        }

        return -1;
    IL_0352:
        return i + 1;
    IL_035a:
        return i + 3;
    IL_0350:
        return i;
    IL_0356:
        return i + 2;
    }

    public static int IndexOfAny<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : IEquatable<T>
    {
        var i = 0;
        if(default(T) != null || value0 != null && value1 != null && value2 != null)
        {
            while(length - i >= 8)
            {
                var other = Unsafe.Add(ref searchSpace, i);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_0471;
                }

                other = Unsafe.Add(ref searchSpace, i + 1);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_0473;
                }

                other = Unsafe.Add(ref searchSpace, i + 2);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_0477;
                }

                other = Unsafe.Add(ref searchSpace, i + 3);
                if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                {
                    other = Unsafe.Add(ref searchSpace, i + 4);
                    if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                    {
                        other = Unsafe.Add(ref searchSpace, i + 5);
                        if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                        {
                            other = Unsafe.Add(ref searchSpace, i + 6);
                            if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                            {
                                other = Unsafe.Add(ref searchSpace, i + 7);
                                if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                                {
                                    i += 8;
                                    continue;
                                }

                                return i + 7;
                            }

                            return i + 6;
                        }

                        return i + 5;
                    }

                    return i + 4;
                }

                goto IL_047b;
            }

            if(length - i >= 4)
            {
                var other = Unsafe.Add(ref searchSpace, i);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_0471;
                }

                other = Unsafe.Add(ref searchSpace, i + 1);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_0473;
                }

                other = Unsafe.Add(ref searchSpace, i + 2);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_0477;
                }

                other = Unsafe.Add(ref searchSpace, i + 3);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_047b;
                }

                i += 4;
            }

            while(i < length)
            {
                var other = Unsafe.Add(ref searchSpace, i);
                if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                {
                    i++;
                    continue;
                }

                goto IL_0471;
            }
        }
        else
        {
            for(i = 0; i < length; i++)
            {
                var other = Unsafe.Add(ref searchSpace, i);
                if(other == null)
                {
                    if(value0 != null && value1 != null && value2 != null)
                    {
                        continue;
                    }
                }
                else if(!other.Equals(value0) && !other.Equals(value1) && !other.Equals(value2))
                {
                    continue;
                }

                goto IL_0471;
            }
        }

        return -1;
    IL_0477:
        return i + 2;
    IL_0471:
        return i;
    IL_047b:
        return i + 3;
    IL_0473:
        return i + 1;
    }

    public static int IndexOfAny<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
    {
        if(valueLength == 0)
        {
            return -1;
        }

        if(typeof(T).IsValueType)
        {
            for(var i = 0; i < searchSpaceLength; i++)
            {
                var other = Unsafe.Add(ref searchSpace, i);
                for(var j = 0; j < valueLength; j++)
                {
                    if(Unsafe.Add(ref value, j).Equals(other))
                    {
                        return i;
                    }
                }
            }
        }
        else
        {
            for(var k = 0; k < searchSpaceLength; k++)
            {
                var val = Unsafe.Add(ref searchSpace, k);
                if(val != null)
                {
                    for(var l = 0; l < valueLength; l++)
                    {
                        if(val.Equals(Unsafe.Add(ref value, l)))
                        {
                            return k;
                        }
                    }

                    continue;
                }

                for(var m = 0; m < valueLength; m++)
                {
                    if(Unsafe.Add(ref value, m) == null)
                    {
                        return k;
                    }
                }
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfAnyChar(ref char searchSpace, char value0, char value1, int length)
    {
        return IndexOfAnyValueType(ref Unsafe.As<char, short>(ref searchSpace), (short)value0, (short)value1, length);
    }

    internal static int IndexOfAnyExcept<T>(ref T searchSpace, T value0, int length)
    {
        for(var i = 0; i < length; i++)
        {
            if(!EqualityComparer<T>.Default.Equals(Unsafe.Add(ref searchSpace, i), value0))
            {
                return i;
            }
        }

        return -1;
    }

    internal static int IndexOfAnyExcept<T>(ref T searchSpace, T value0, T value1, int length)
    {
        for(var i = 0; i < length; i++)
        {
            ref var reference = ref Unsafe.Add(ref searchSpace, i);
            if(!EqualityComparer<T>.Default.Equals(reference, value0) && !EqualityComparer<T>.Default.Equals(reference, value1))
            {
                return i;
            }
        }

        return -1;
    }

    internal static int IndexOfAnyExcept<T>(ref T searchSpace, T value0, T value1, T value2, int length)
    {
        for(var i = 0; i < length; i++)
        {
            ref var reference = ref Unsafe.Add(ref searchSpace, i);
            if(!EqualityComparer<T>.Default.Equals(reference, value0) && !EqualityComparer<T>.Default.Equals(reference, value1) && !EqualityComparer<T>.Default.Equals(reference, value2))
            {
                return i;
            }
        }

        return -1;
    }

    internal static int IndexOfAnyExcept<T>(ref T searchSpace, T value0, T value1, T value2, T value3, int length)
    {
        for(var i = 0; i < length; i++)
        {
            ref var reference = ref Unsafe.Add(ref searchSpace, i);
            if(!EqualityComparer<T>.Default.Equals(reference, value0) && !EqualityComparer<T>.Default.Equals(reference, value1) && !EqualityComparer<T>.Default.Equals(reference, value2) && !EqualityComparer<T>.Default.Equals(reference, value3))
            {
                return i;
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfAnyExceptValueType<T>(ref T searchSpace, T value, int length) where T : struct, INumber<T>
    {
        return IndexOfValueType<T, Negate<T>>(ref searchSpace, value, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfAnyExceptValueType<T>(ref T searchSpace, T value0, T value1, int length) where T : struct, INumber<T>
    {
        return IndexOfAnyValueType<T, Negate<T>>(ref searchSpace, value0, value1, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfAnyExceptValueType<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : struct, INumber<T>
    {
        return IndexOfAnyValueType<T, Negate<T>>(ref searchSpace, value0, value1, value2, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfAnyExceptValueType<T>(ref T searchSpace, T value0, T value1, T value2, T value3, int length) where T : struct, INumber<T>
    {
        return IndexOfAnyValueType<T, Negate<T>>(ref searchSpace, value0, value1, value2, value3, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfAnyExceptValueType<T>(ref T searchSpace, T value0, T value1, T value2, T value3, T value4, int length) where T : struct, INumber<T>
            => IndexOfAnyValueType<T, Negate<T>>(ref searchSpace, value0, value1, value2, value3, value4, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfAnyValueType<T>(ref T searchSpace, T value0, T value1, int length) where T : struct, INumber<T>
    {
        return IndexOfAnyValueType<T, DontNegate<T>>(ref searchSpace, value0, value1, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfAnyValueType<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : struct, INumber<T>
    {
        return IndexOfAnyValueType<T, DontNegate<T>>(ref searchSpace, value0, value1, value2, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfAnyValueType<T>(ref T searchSpace, T value0, T value1, T value2, T value3, int length) where T : struct, INumber<T>
    {
        return IndexOfAnyValueType<T, DontNegate<T>>(ref searchSpace, value0, value1, value2, value3, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static int IndexOfAnyValueType<T>(ref T searchSpace, T value0, T value1, T value2, T value3, T value4, int length) where T : struct, INumber<T>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<T>.Count)
        {
            nuint num = 0u;
            while(length >= 4)
            {
                length -= 4;
                ref var reference = ref Unsafe.Add(ref searchSpace, num);
                var val = reference;
                if(val == value0 || val == value1 || val == value2 || val == value3 || val == value4)
                {
                    return (int)num;
                }

                val = Unsafe.Add(ref reference, 1);
                if(val == value0 || val == value1 || val == value2 || val == value3 || val == value4)
                {
                    return (int)num + 1;
                }

                val = Unsafe.Add(ref reference, 2);
                if(val == value0 || val == value1 || val == value2 || val == value3 || val == value4)
                {
                    return (int)num + 2;
                }

                val = Unsafe.Add(ref reference, 3);
                if(val == value0 || val == value1 || val == value2 || val == value3 || val == value4)
                {
                    return (int)num + 3;
                }

                num += 4;
            }

            while(length > 0)
            {
                length--;
                var val = Unsafe.Add(ref searchSpace, num);
                if(val == value0 || val == value1 || val == value2 || val == value3 || val == value4)
                {
                    return (int)num;
                }

                num++;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<T>.Count)
        {
            var left = Vector256.Create(value0);
            var left2 = Vector256.Create(value1);
            var left3 = Vector256.Create(value2);
            var left4 = Vector256.Create(value3);
            var left5 = Vector256.Create(value4);
            ref var reference2 = ref searchSpace;
            ref var reference3 = ref Unsafe.Add(ref searchSpace, length - Vector256<T>.Count);
            do
            {
                var right = Vector256.LoadUnsafe(ref reference2);
                var vector = Vector256.Equals(left, right) | Vector256.Equals(left2, right) | Vector256.Equals(left3, right) | Vector256.Equals(left4, right) | Vector256.Equals(left5, right);
                if(vector == Vector256<T>.Zero)
                {
                    reference2 = ref Unsafe.Add(ref reference2, Vector256<T>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference2, vector);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference2, ref reference3));
            if((uint)length % Vector256<T>.Count != 0L)
            {
                var right = Vector256.LoadUnsafe(ref reference3);
                var vector = Vector256.Equals(left, right) | Vector256.Equals(left2, right) | Vector256.Equals(left3, right) | Vector256.Equals(left4, right) | Vector256.Equals(left5, right);
                if(vector != Vector256<T>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference3, vector);
                }
            }
        }
        else
        {
            var left6 = Vector128.Create(value0);
            var left7 = Vector128.Create(value1);
            var left8 = Vector128.Create(value2);
            var left9 = Vector128.Create(value3);
            var left10 = Vector128.Create(value4);
            ref var reference4 = ref searchSpace;
            ref var reference5 = ref Unsafe.Add(ref searchSpace, length - Vector128<T>.Count);
            do
            {
                var right2 = Vector128.LoadUnsafe(ref reference4);
                var vector2 = Vector128.Equals(left6, right2) | Vector128.Equals(left7, right2) | Vector128.Equals(left8, right2) | Vector128.Equals(left9, right2) | Vector128.Equals(left10, right2);
                if(vector2 == Vector128<T>.Zero)
                {
                    reference4 = ref Unsafe.Add(ref reference4, Vector128<T>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference4, vector2);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference4, ref reference5));
            if((uint)length % Vector128<T>.Count != 0L)
            {
                var right2 = Vector128.LoadUnsafe(ref reference5);
                var vector2 = Vector128.Equals(left6, right2) | Vector128.Equals(left7, right2) | Vector128.Equals(left8, right2) | Vector128.Equals(left9, right2) | Vector128.Equals(left10, right2);
                if(vector2 != Vector128<T>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference5, vector2);
                }
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int IndexOfAnyValueType<TValue, TNegator>(ref TValue searchSpace, TValue value0, TValue value1, TValue value2, TValue value3, TValue value4, int length)
            where TValue : struct, INumber<TValue>
            where TNegator : struct, INegator<TValue>
    {
        Debug.Assert(length >= 0, "Expected non-negative length");
        Debug.Assert(value0 is byte or short or int or long, "Expected caller to normalize to one of these types");

        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            nuint offset = 0;
            TValue lookUp;

            while(length >= 4)
            {
                length -= 4;

                ref TValue current = ref Unsafe.Add(ref searchSpace, offset);
                lookUp = current;
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    goto Found;
                lookUp = Unsafe.Add(ref current, 1);
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    goto Found1;
                lookUp = Unsafe.Add(ref current, 2);
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    goto Found2;
                lookUp = Unsafe.Add(ref current, 3);
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    goto Found3;

                offset += 4;
            }

            while(length > 0)
            {
                length -= 1;

                lookUp = Unsafe.Add(ref searchSpace, offset);
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    goto Found;

                offset += 1;
            }

            return -1;
        Found3:
            return (int)(offset + 3);
        Found2:
            return (int)(offset + 2);
        Found1:
            return (int)(offset + 1);
        Found:
            return (int)(offset);
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            Vector256<TValue> equals, current, values0 = Vector256.Create(value0), values1 = Vector256.Create(value1),
                values2 = Vector256.Create(value2), values3 = Vector256.Create(value3), values4 = Vector256.Create(value4);
            ref TValue currentSearchSpace = ref searchSpace;
            ref TValue oneVectorAwayFromEnd = ref Unsafe.Add(ref searchSpace, (uint)(length - Vector256<TValue>.Count));

            // Loop until either we've finished all elements or there's less than a vector's-worth remaining.
            do
            {
                current = Vector256.LoadUnsafe(ref currentSearchSpace);
                equals = TNegator.NegateIfNeeded(Vector256.Equals(values0, current) | Vector256.Equals(values1, current) | Vector256.Equals(values2, current)
                       | Vector256.Equals(values3, current) | Vector256.Equals(values4, current));
                if(equals == Vector256<TValue>.Zero)
                {
                    currentSearchSpace = ref Unsafe.Add(ref currentSearchSpace, Vector256<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref currentSearchSpace, equals);
            }
            while(!Unsafe.IsAddressGreaterThan(ref currentSearchSpace, ref oneVectorAwayFromEnd));

            // If any elements remain, process the last vector in the search space.
            if((uint)length % Vector256<TValue>.Count != 0)
            {
                current = Vector256.LoadUnsafe(ref oneVectorAwayFromEnd);
                equals = TNegator.NegateIfNeeded(Vector256.Equals(values0, current) | Vector256.Equals(values1, current) | Vector256.Equals(values2, current)
                       | Vector256.Equals(values3, current) | Vector256.Equals(values4, current));
                if(equals != Vector256<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref oneVectorAwayFromEnd, equals);
                }
            }
        }
        else
        {
            Vector128<TValue> equals, current, values0 = Vector128.Create(value0), values1 = Vector128.Create(value1),
                values2 = Vector128.Create(value2), values3 = Vector128.Create(value3), values4 = Vector128.Create(value4);
            ref TValue currentSearchSpace = ref searchSpace;
            ref TValue oneVectorAwayFromEnd = ref Unsafe.Add(ref searchSpace, (uint)(length - Vector128<TValue>.Count));

            // Loop until either we've finished all elements or there's less than a vector's-worth remaining.
            do
            {
                current = Vector128.LoadUnsafe(ref currentSearchSpace);
                equals = TNegator.NegateIfNeeded(Vector128.Equals(values0, current) | Vector128.Equals(values1, current) | Vector128.Equals(values2, current)
                       | Vector128.Equals(values3, current) | Vector128.Equals(values4, current));
                if(equals == Vector128<TValue>.Zero)
                {
                    currentSearchSpace = ref Unsafe.Add(ref currentSearchSpace, Vector128<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref currentSearchSpace, equals);
            }
            while(!Unsafe.IsAddressGreaterThan(ref currentSearchSpace, ref oneVectorAwayFromEnd));

            // If any elements remain, process the first vector in the search space.
            if((uint)length % Vector128<TValue>.Count != 0)
            {
                current = Vector128.LoadUnsafe(ref oneVectorAwayFromEnd);
                equals = TNegator.NegateIfNeeded(Vector128.Equals(values0, current) | Vector128.Equals(values1, current) | Vector128.Equals(values2, current)
                       | Vector128.Equals(values3, current) | Vector128.Equals(values4, current));
                if(equals != Vector128<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref oneVectorAwayFromEnd, equals);
                }
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static unsafe int IndexOfNullByte(ref byte searchSpace)
    {
        nuint num = 0u;
        nuint num2 = 2147483647u;
        if(Vector128.IsHardwareAccelerated)
        {
            num2 = UnalignedCountVector128(ref searchSpace);
        }
        else if(Vector.IsHardwareAccelerated)
        {
            num2 = UnalignedCountVector(ref searchSpace);
        }

        while(true)
        {
            if(num2 >= 8)
            {
                num2 -= 8;
                if(Unsafe.AddByteOffset(ref searchSpace, num) != 0)
                {
                    if(Unsafe.AddByteOffset(ref searchSpace, num + 1) == 0)
                    {
                        goto IL_0335;
                    }

                    if(Unsafe.AddByteOffset(ref searchSpace, num + 2) == 0)
                    {
                        goto IL_033b;
                    }

                    if(Unsafe.AddByteOffset(ref searchSpace, num + 3) != 0)
                    {
                        if(Unsafe.AddByteOffset(ref searchSpace, num + 4) != 0)
                        {
                            if(Unsafe.AddByteOffset(ref searchSpace, num + 5) != 0)
                            {
                                if(Unsafe.AddByteOffset(ref searchSpace, num + 6) != 0)
                                {
                                    if(Unsafe.AddByteOffset(ref searchSpace, num + 7) == 0)
                                    {
                                        break;
                                    }

                                    num += 8;
                                    continue;
                                }

                                return (int)(num + 6);
                            }

                            return (int)(num + 5);
                        }

                        return (int)(num + 4);
                    }

                    goto IL_0341;
                }
            }
            else
            {
                if(num2 >= 4)
                {
                    num2 -= 4;
                    if(Unsafe.AddByteOffset(ref searchSpace, num) == 0)
                    {
                        goto IL_0332;
                    }

                    if(Unsafe.AddByteOffset(ref searchSpace, num + 1) == 0)
                    {
                        goto IL_0335;
                    }

                    if(Unsafe.AddByteOffset(ref searchSpace, num + 2) == 0)
                    {
                        goto IL_033b;
                    }

                    if(Unsafe.AddByteOffset(ref searchSpace, num + 3) == 0)
                    {
                        goto IL_0341;
                    }

                    num += 4;
                }

                while(num2 != 0)
                {
                    num2--;
                    if(Unsafe.AddByteOffset(ref searchSpace, num) != 0)
                    {
                        num++;
                        continue;
                    }

                    goto IL_0332;
                }

                if(Vector256.IsHardwareAccelerated)
                {
                    if(num < int.MaxValue)
                    {
                        if((((uint)Unsafe.AsPointer(ref searchSpace) + num) & (nuint)(Vector256<byte>.Count - 1)) != 0)
                        {
                            var right = Vector128.LoadUnsafe(ref searchSpace, num);
                            var num3 = Vector128.Equals(Vector128<byte>.Zero, right).ExtractMostSignificantBits();
                            if(num3 != 0)
                            {
                                return (int)(num + (uint)BitOperations.TrailingZeroCount(num3));
                            }

                            num += (nuint)Vector128<byte>.Count;
                        }

                        num2 = GetByteVector256SpanLength(num, int.MaxValue);
                        if(num2 > num)
                        {
                            do
                            {
                                var right2 = Vector256.LoadUnsafe(ref searchSpace, num);
                                var num4 = Vector256.Equals(Vector256<byte>.Zero, right2).ExtractMostSignificantBits();
                                if(num4 == 0)
                                {
                                    num += (nuint)Vector256<byte>.Count;
                                    continue;
                                }

                                return (int)(num + (uint)BitOperations.TrailingZeroCount(num4));
                            }
                            while(num2 > num);
                        }

                        num2 = GetByteVector128SpanLength(num, int.MaxValue);
                        if(num2 > num)
                        {
                            var right3 = Vector128.LoadUnsafe(ref searchSpace, num);
                            var num5 = Vector128.Equals(Vector128<byte>.Zero, right3).ExtractMostSignificantBits();
                            if(num5 != 0)
                            {
                                return (int)(num + (uint)BitOperations.TrailingZeroCount(num5));
                            }

                            num += (nuint)Vector128<byte>.Count;
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
                        for(num2 = GetByteVector128SpanLength(num, int.MaxValue); num2 > num; num += (nuint)Vector128<byte>.Count)
                        {
                            var right4 = Vector128.LoadUnsafe(ref searchSpace, num);
                            var vector = Vector128.Equals(Vector128<byte>.Zero, right4);
                            if(!(vector == Vector128<byte>.Zero))
                            {
                                var value = vector.ExtractMostSignificantBits();
                                return (int)(num + (uint)BitOperations.TrailingZeroCount(value));
                            }
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
                    for(num2 = GetByteVectorSpanLength(num, int.MaxValue); num2 > num; num += (nuint)Vector<byte>.Count)
                    {
                        var vector2 = Vector.Equals(Vector<byte>.Zero, LoadVector(ref searchSpace, num));
                        if(!Vector<byte>.Zero.Equals(vector2))
                        {
                            return (int)num + LocateFirstFoundByte(vector2);
                        }
                    }

                    if(num < int.MaxValue)
                    {
                        num2 = int.MaxValue - num;
                        continue;
                    }
                }

                ThrowMustBeNullTerminatedString();
            }

            goto IL_0332;
        IL_0332:
            return (int)num;
        IL_0335:
            return (int)(num + 1);
        IL_0341:
            return (int)(num + 3);
        IL_033b:
            return (int)(num + 2);
        }

        return (int)(num + 7);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfValueType<T>(ref T searchSpace, T value, int length) where T : struct, INumber<T>
    {
        return IndexOfValueType<T, DontNegate<T>>(ref searchSpace, value, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int IndexOfAnyValueType<TValue, TNegator>(ref TValue searchSpace, TValue value0, TValue value1, int length) where TValue : struct, INumber<TValue> where TNegator : struct, INegator<TValue>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            nuint num = 0u;
            if(typeof(TValue) == typeof(byte))
            {
                while(length >= 8)
                {
                    length -= 8;
                    ref var reference = ref Unsafe.Add(ref searchSpace, num);
                    var val = reference;
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num;
                    }

                    val = Unsafe.Add(ref reference, 1);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num + 1;
                    }

                    val = Unsafe.Add(ref reference, 2);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num + 2;
                    }

                    val = Unsafe.Add(ref reference, 3);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num + 3;
                    }

                    val = Unsafe.Add(ref reference, 4);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num + 4;
                    }

                    val = Unsafe.Add(ref reference, 5);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num + 5;
                    }

                    val = Unsafe.Add(ref reference, 6);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num + 6;
                    }

                    val = Unsafe.Add(ref reference, 7);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num + 7;
                    }

                    num += 8;
                }
            }

            while(length >= 4)
            {
                length -= 4;
                ref var reference2 = ref Unsafe.Add(ref searchSpace, num);
                var val = reference2;
                if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                {
                    return (int)num;
                }

                val = Unsafe.Add(ref reference2, 1);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                {
                    return (int)num + 1;
                }

                val = Unsafe.Add(ref reference2, 2);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                {
                    return (int)num + 2;
                }

                val = Unsafe.Add(ref reference2, 3);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                {
                    return (int)num + 3;
                }

                num += 4;
            }

            while(length > 0)
            {
                length--;
                var val = Unsafe.Add(ref searchSpace, num);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                {
                    return (int)num;
                }

                num++;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            var left = Vector256.Create(value0);
            var left2 = Vector256.Create(value1);
            ref var reference3 = ref searchSpace;
            ref var reference4 = ref Unsafe.Add(ref searchSpace, length - Vector256<TValue>.Count);
            do
            {
                var right = Vector256.LoadUnsafe(ref reference3);
                var vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left2, right));
                if(vector == Vector256<TValue>.Zero)
                {
                    reference3 = ref Unsafe.Add(ref reference3, Vector256<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference3, vector);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference3, ref reference4));
            if((uint)length % Vector256<TValue>.Count != 0L)
            {
                var right = Vector256.LoadUnsafe(ref reference4);
                var vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left2, right));
                if(vector != Vector256<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference4, vector);
                }
            }
        }
        else
        {
            var left3 = Vector128.Create(value0);
            var left4 = Vector128.Create(value1);
            ref var reference5 = ref searchSpace;
            ref var reference6 = ref Unsafe.Add(ref searchSpace, length - Vector128<TValue>.Count);
            do
            {
                var right2 = Vector128.LoadUnsafe(ref reference5);
                var vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left3, right2) | Vector128.Equals(left4, right2));
                if(vector2 == Vector128<TValue>.Zero)
                {
                    reference5 = ref Unsafe.Add(ref reference5, Vector128<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference5, vector2);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference5, ref reference6));
            if((uint)length % Vector128<TValue>.Count != 0L)
            {
                var right2 = Vector128.LoadUnsafe(ref reference6);
                var vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left3, right2) | Vector128.Equals(left4, right2));
                if(vector2 != Vector128<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference6, vector2);
                }
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int IndexOfAnyValueType<TValue, TNegator>(ref TValue searchSpace, TValue value0, TValue value1, TValue value2, int length) where TValue : struct, INumber<TValue> where TNegator : struct, INegator<TValue>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            nuint num = 0u;
            if(typeof(TValue) == typeof(byte))
            {
                while(length >= 8)
                {
                    length -= 8;
                    ref var reference = ref Unsafe.Add(ref searchSpace, num);
                    var val = reference;
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num;
                    }

                    val = Unsafe.Add(ref reference, 1);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num + 1;
                    }

                    val = Unsafe.Add(ref reference, 2);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num + 2;
                    }

                    val = Unsafe.Add(ref reference, 3);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num + 3;
                    }

                    val = Unsafe.Add(ref reference, 4);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num + 4;
                    }

                    val = Unsafe.Add(ref reference, 5);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num + 5;
                    }

                    val = Unsafe.Add(ref reference, 6);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num + 6;
                    }

                    val = Unsafe.Add(ref reference, 7);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num + 7;
                    }

                    num += 8;
                }
            }

            while(length >= 4)
            {
                length -= 4;
                ref var reference2 = ref Unsafe.Add(ref searchSpace, num);
                var val = reference2;
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                {
                    return (int)num;
                }

                val = Unsafe.Add(ref reference2, 1);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                {
                    return (int)num + 1;
                }

                val = Unsafe.Add(ref reference2, 2);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                {
                    return (int)num + 2;
                }

                val = Unsafe.Add(ref reference2, 3);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                {
                    return (int)num + 3;
                }

                num += 4;
            }

            while(length > 0)
            {
                length--;
                var val = Unsafe.Add(ref searchSpace, num);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                {
                    return (int)num;
                }

                num++;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            var left = Vector256.Create(value0);
            var left2 = Vector256.Create(value1);
            var left3 = Vector256.Create(value2);
            ref var reference3 = ref searchSpace;
            ref var reference4 = ref Unsafe.Add(ref searchSpace, length - Vector256<TValue>.Count);
            do
            {
                var right = Vector256.LoadUnsafe(ref reference3);
                var vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left2, right) | Vector256.Equals(left3, right));
                if(vector == Vector256<TValue>.Zero)
                {
                    reference3 = ref Unsafe.Add(ref reference3, Vector256<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference3, vector);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference3, ref reference4));
            if((uint)length % Vector256<TValue>.Count != 0L)
            {
                var right = Vector256.LoadUnsafe(ref reference4);
                var vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left2, right) | Vector256.Equals(left3, right));
                if(vector != Vector256<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference4, vector);
                }
            }
        }
        else
        {
            var left4 = Vector128.Create(value0);
            var left5 = Vector128.Create(value1);
            var left6 = Vector128.Create(value2);
            ref var reference5 = ref searchSpace;
            ref var reference6 = ref Unsafe.Add(ref searchSpace, length - Vector128<TValue>.Count);
            do
            {
                var right2 = Vector128.LoadUnsafe(ref reference5);
                var vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left4, right2) | Vector128.Equals(left5, right2) | Vector128.Equals(left6, right2));
                if(vector2 == Vector128<TValue>.Zero)
                {
                    reference5 = ref Unsafe.Add(ref reference5, Vector128<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference5, vector2);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference5, ref reference6));
            if((uint)length % Vector128<TValue>.Count != 0L)
            {
                var right2 = Vector128.LoadUnsafe(ref reference6);
                var vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left4, right2) | Vector128.Equals(left5, right2) | Vector128.Equals(left6, right2));
                if(vector2 != Vector128<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference6, vector2);
                }
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int IndexOfAnyValueType<TValue, TNegator>(ref TValue searchSpace, TValue value0, TValue value1, TValue value2, TValue value3, int length) where TValue : struct, INumber<TValue> where TNegator : struct, INegator<TValue>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            nuint num = 0u;
            while(length >= 4)
            {
                length -= 4;
                ref var reference = ref Unsafe.Add(ref searchSpace, num);
                var val = reference;
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num;
                }

                val = Unsafe.Add(ref reference, 1);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num + 1;
                }

                val = Unsafe.Add(ref reference, 2);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num + 2;
                }

                val = Unsafe.Add(ref reference, 3);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num + 3;
                }

                num += 4;
            }

            while(length > 0)
            {
                length--;
                var val = Unsafe.Add(ref searchSpace, num);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num;
                }

                num++;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            var left = Vector256.Create(value0);
            var left2 = Vector256.Create(value1);
            var left3 = Vector256.Create(value2);
            var left4 = Vector256.Create(value3);
            ref var reference2 = ref searchSpace;
            ref var reference3 = ref Unsafe.Add(ref searchSpace, length - Vector256<TValue>.Count);
            do
            {
                var right = Vector256.LoadUnsafe(ref reference2);
                var vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left2, right) | Vector256.Equals(left3, right) | Vector256.Equals(left4, right));
                if(vector == Vector256<TValue>.Zero)
                {
                    reference2 = ref Unsafe.Add(ref reference2, Vector256<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference2, vector);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference2, ref reference3));
            if((uint)length % Vector256<TValue>.Count != 0L)
            {
                var right = Vector256.LoadUnsafe(ref reference3);
                var vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left2, right) | Vector256.Equals(left3, right) | Vector256.Equals(left4, right));
                if(vector != Vector256<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference3, vector);
                }
            }
        }
        else
        {
            var left5 = Vector128.Create(value0);
            var left6 = Vector128.Create(value1);
            var left7 = Vector128.Create(value2);
            var left8 = Vector128.Create(value3);
            ref var reference4 = ref searchSpace;
            ref var reference5 = ref Unsafe.Add(ref searchSpace, length - Vector128<TValue>.Count);
            do
            {
                var right2 = Vector128.LoadUnsafe(ref reference4);
                var vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left5, right2) | Vector128.Equals(left6, right2) | Vector128.Equals(left7, right2) | Vector128.Equals(left8, right2));
                if(vector2 == Vector128<TValue>.Zero)
                {
                    reference4 = ref Unsafe.Add(ref reference4, Vector128<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference4, vector2);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference4, ref reference5));
            if((uint)length % Vector128<TValue>.Count != 0L)
            {
                var right2 = Vector128.LoadUnsafe(ref reference5);
                var vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left5, right2) | Vector128.Equals(left6, right2) | Vector128.Equals(left7, right2) | Vector128.Equals(left8, right2));
                if(vector2 != Vector128<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference5, vector2);
                }
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int IndexOfValueTypeCore<TValue>(ref TValue searchSpace, TValue value, int length)
       where TValue : struct, INumber<TValue>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            nuint num = 0u;
            while(length >= 8)
            {
                length -= 8;
                if(Unsafe.Add(ref searchSpace, num) == value)
                {
                    return (int)num;
                }

                if(Unsafe.Add(ref searchSpace, num + 1) == value)
                {
                    return (int)num + 1;
                }

                if(Unsafe.Add(ref searchSpace, num + 2) == value)
                {
                    return (int)num + 2;
                }

                if(Unsafe.Add(ref searchSpace, num + 3) == value)
                {
                    return (int)num + 3;
                }

                if(Unsafe.Add(ref searchSpace, num + 4) == value)
                {
                    return (int)num + 4;
                }

                if(Unsafe.Add(ref searchSpace, num + 5) == value)
                {
                    return (int)num + 5;
                }

                if(Unsafe.Add(ref searchSpace, num + 6) == value)
                {
                    return (int)num + 6;
                }

                if(Unsafe.Add(ref searchSpace, num + 7) == value)
                {
                    return (int)num + 7;
                }

                num += 8;
            }

            if(length >= 4)
            {
                length -= 4;
                if(Unsafe.Add(ref searchSpace, num) == value)
                {
                    return (int)num;
                }

                if(Unsafe.Add(ref searchSpace, num + 1) == value)
                {
                    return (int)num + 1;
                }

                if(Unsafe.Add(ref searchSpace, num + 2) == value)
                {
                    return (int)num + 2;
                }

                if(Unsafe.Add(ref searchSpace, num + 3) == value)
                {
                    return (int)num + 3;
                }

                num += 4;
            }

            while(length > 0)
            {
                length--;
                if(Unsafe.Add(ref searchSpace, num) == value)
                {
                    return (int)num;
                }

                num++;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            var vector1 = Vector256.Create(value);
            ref var reference = ref searchSpace;
            ref var reference2 = ref Unsafe.Add(ref searchSpace, length - Vector256<TValue>.Count);
            do
            {
                var vector =Vector256.Equals(vector1, Vector256.LoadUnsafe(ref reference));
                if(vector == Vector256<TValue>.Zero)
                {
                    reference = ref Unsafe.Add(ref reference, Vector256<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference, vector);
            }
            while(Unsafe.IsAddressLessThan(ref reference, ref reference2));

            if((uint)length % Vector256<TValue>.Count != 0L)
            {
                var vector =Vector256.Equals(vector1, Vector256.LoadUnsafe(ref reference2));
                if(vector != Vector256<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference2, vector);
                }
            }
        }
        else
        {
            var left2 = Vector128.Create(value);
            ref var reference3 = ref searchSpace;
            ref var reference4 = ref Unsafe.Add(ref searchSpace, length - Vector128<TValue>.Count);
            do
            {
                var vector = Vector128.Equals(left2, Vector128.LoadUnsafe(ref reference3));
                if(vector == Vector128<TValue>.Zero)
                {
                    reference3 = ref Unsafe.Add(ref reference3, Vector128<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference3, vector);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference3, ref reference4));
            if((uint)length % Vector128<TValue>.Count != 0L)
            {
                var vector2 = Vector128.Equals(left2, Vector128.LoadUnsafe(ref reference4));
                if(vector2 != Vector128<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference4, vector2);
                }
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int IndexOfValueType<TValue, TNegator>(ref TValue searchSpace, TValue value, int length)
        where TValue : struct, INumber<TValue> where TNegator : struct, INegator<TValue>
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
            var vector1 = Vector256.Create(value);
            ref var reference = ref searchSpace;
            ref var reference2 = ref Unsafe.Add(ref searchSpace, length - Vector256<TValue>.Count);
            do
            {
                var vector = TNegator.NegateIfNeeded(Vector256.Equals(vector1, Vector256.LoadUnsafe(ref reference)));
                if(vector == Vector256<TValue>.Zero)
                {
                    reference = ref Unsafe.Add(ref reference, Vector256<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference, vector);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference, ref reference2));
            if((uint)length % Vector256<TValue>.Count != 0L)
            {
                var vector = TNegator.NegateIfNeeded(Vector256.Equals(vector1, Vector256.LoadUnsafe(ref reference2)));
                if(vector != Vector256<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference2, vector);
                }
            }
        }
        else
        {
            var left2 = Vector128.Create(value);
            ref var reference3 = ref searchSpace;
            ref var reference4 = ref Unsafe.Add(ref searchSpace, length - Vector128<TValue>.Count);
            do
            {
                var vector = TNegator.NegateIfNeeded(Vector128.Equals(left2, Vector128.LoadUnsafe(ref reference3)));
                if(vector == Vector128<TValue>.Zero)
                {
                    reference3 = ref Unsafe.Add(ref reference3, Vector128<TValue>.Count);
                    continue;
                }

                return ComputeFirstIndex(ref searchSpace, ref reference3, vector);
            }
            while(!Unsafe.IsAddressGreaterThan(ref reference3, ref reference4));
            if((uint)length % Vector128<TValue>.Count != 0L)
            {
                var vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, Vector128.LoadUnsafe(ref reference4)));
                if(vector2 != Vector128<TValue>.Zero)
                {
                    return ComputeFirstIndex(ref searchSpace, ref reference4, vector2);
                }
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ComputeFirstIndex<T>(ref T searchSpace, ref T current, Vector128<T> equals) where T : struct
    {
        var value = equals.ExtractMostSignificantBits();
        var num = BitOperations.TrailingZeroCount(value);
        return num + (int)(Unsafe.ByteOffset(ref searchSpace, ref current) / Unsafe.SizeOf<T>());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ComputeFirstIndex<T>(ref T searchSpace, ref T current, Vector256<T> equals) where T : struct
    {
        var value = Vector256.ExtractMostSignificantBits(equals);

        var num = BitOperations.TrailingZeroCount(value);

        return num + (int)(Unsafe.ByteOffset(ref searchSpace, ref current) / Unsafe.SizeOf<T>());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nuint GetByteVector128SpanLength(nuint offset, int length)
    {
        return (uint)((length - (int)offset) & ~(Vector128<byte>.Count - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nuint GetByteVector256SpanLength(nuint offset, int length)
    {
        return (uint)((length - (int)offset) & ~(Vector256<byte>.Count - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nuint GetByteVectorSpanLength(nuint offset, int length)
    {
        return (uint)((length - (int)offset) & ~(Vector<byte>.Count - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LocateFirstFoundByte(Vector<byte> match)
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

        return i * 8 + LocateFirstFoundByte(num);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LocateFirstFoundByte(ulong match)
    {
        return BitOperations.TrailingZeroCount(match) >> 3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe nuint UnalignedCountVector(ref byte searchSpace)
    {
        var num = (nint)Unsafe.AsPointer(ref searchSpace) & Vector<byte>.Count - 1;
        return (nuint)((Vector<byte>.Count - num) & (Vector<byte>.Count - 1));
    }
}
