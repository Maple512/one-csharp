namespace OneI.Buffers;

using System.Numerics;
using System.Runtime.Intrinsics;

internal static partial class ValueHelper
{
    public static int LastIndexOf(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
    {
        if(valueLength == 0)
        {
            return searchSpaceLength;
        }

        var num = valueLength - 1;
        if(num == 0)
        {
            return LastIndexOfValueType(ref searchSpace, value, searchSpaceLength);
        }

        var num2 = 0;
        var value2 = value;
        var num3 = searchSpaceLength - num;
        if(!Vector128.IsHardwareAccelerated || num3 < Vector128<byte>.Count)
        {
            ref var second = ref Unsafe.Add(ref value, 1);
            while(true)
            {
                var num4 = searchSpaceLength - num2 - num;
                if(num4 <= 0)
                {
                    break;
                }

                var num5 = LastIndexOfValueType(ref searchSpace, value2, num4);
                if(num5 < 0)
                {
                    break;
                }

                if(SequenceEqual(ref Unsafe.Add(ref searchSpace, num5 + 1), ref second, (uint)num))
                {
                    return num5;
                }

                num2 += num4 - num5;
            }

            return -1;
        }

        if(Vector256.IsHardwareAccelerated && num3 >= Vector256<byte>.Count)
        {
            num2 = num3 - Vector256<byte>.Count;
            var b = Unsafe.Add(ref value, num);
            var num6 = num;
            while(b == value && num6 > 1)
            {
                b = Unsafe.Add(ref value, --num6);
            }

            var left = Vector256.Create(value);
            var left2 = Vector256.Create(b);
            while(true)
            {
                var vector = Vector256.Equals(left, Vector256.LoadUnsafe(ref searchSpace, (nuint)num2));
                var vector2 = Vector256.Equals(left2, Vector256.LoadUnsafe(ref searchSpace, (nuint)(num2 + num6)));
                var vector3 = (vector & vector2).AsByte();
                if(vector3 != Vector256<byte>.Zero)
                {
                    var num7 = vector3.ExtractMostSignificantBits();
                    do
                    {
                        var num8 = 31 - BitOperations.LeadingZeroCount(num7);
                        if(valueLength == 2 || SequenceEqual(ref Unsafe.Add(ref searchSpace, num2 + num8), ref value, (uint)valueLength))
                        {
                            return num8 + num2;
                        }

                        num7 = ResetBit(num7, num8);
                    }
                    while(num7 != 0);
                }

                num2 -= Vector256<byte>.Count;
                if(num2 == -Vector256<byte>.Count)
                {
                    break;
                }

                if(num2 < 0)
                {
                    num2 = 0;
                }
            }

            return -1;
        }

        num2 = num3 - Vector128<byte>.Count;
        var b2 = Unsafe.Add(ref value, num);
        var num9 = num;
        while(b2 == value && num9 > 1)
        {
            b2 = Unsafe.Add(ref value, --num9);
        }

        var left3 = Vector128.Create(value);
        var left4 = Vector128.Create(b2);
        while(true)
        {
            var vector4 = Vector128.Equals(left3, Vector128.LoadUnsafe(ref searchSpace, (nuint)num2));
            var vector5 = Vector128.Equals(left4, Vector128.LoadUnsafe(ref searchSpace, (nuint)(num2 + num9)));
            var vector6 = (vector4 & vector5).AsByte();
            if(vector6 != Vector128<byte>.Zero)
            {
                var num10 = vector6.ExtractMostSignificantBits();
                do
                {
                    var num11 = 31 - BitOperations.LeadingZeroCount(num10);
                    if(valueLength == 2 || SequenceEqual(ref Unsafe.Add(ref searchSpace, num2 + num11), ref value, (uint)valueLength))
                    {
                        return num11 + num2;
                    }

                    num10 = ResetBit(num10, num11);
                }
                while(num10 != 0);
            }

            num2 -= Vector128<byte>.Count;
            if(num2 == -Vector128<byte>.Count)
            {
                break;
            }

            if(num2 < 0)
            {
                num2 = 0;
            }
        }

        return -1;
    }

    public static int LastIndexOf(ref char searchSpace, int searchSpaceLength, ref char value, int valueLength)
    {
        if(valueLength == 0)
        {
            return searchSpaceLength;
        }

        var num = valueLength - 1;
        if(num == 0)
        {
            return LastIndexOfValueType(ref Unsafe.As<char, short>(ref searchSpace), (short)value, searchSpaceLength);
        }

        var num2 = 0;
        var c = value;
        var num3 = searchSpaceLength - num;
        if(!Vector128.IsHardwareAccelerated || num3 < Vector128<ushort>.Count)
        {
            ref var second = ref Unsafe.As<char, byte>(ref Unsafe.Add(ref value, 1));
            while(true)
            {
                var num4 = searchSpaceLength - num2 - num;
                if(num4 <= 0)
                {
                    break;
                }

                var num5 = LastIndexOfValueType(ref Unsafe.As<char, short>(ref searchSpace), (short)c, num4);
                if(num5 == -1)
                {
                    break;
                }

                if(SequenceEqual(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref searchSpace, num5 + 1)), ref second, (uint)num * (nuint)2u))
                {
                    return num5;
                }

                num2 += num4 - num5;
            }

            return -1;
        }

        ref var source = ref Unsafe.As<char, ushort>(ref searchSpace);
        if(Vector256.IsHardwareAccelerated && num3 >= Vector256<ushort>.Count)
        {
            num2 = num3 - Vector256<ushort>.Count;
            var c2 = Unsafe.Add(ref value, num);
            var num6 = num;
            while(c2 == c && num6 > 1)
            {
                c2 = Unsafe.Add(ref value, --num6);
            }

            var left = Vector256.Create((ushort)c);
            var left2 = Vector256.Create((ushort)c2);
            while(true)
            {
                var vector = Vector256.Equals(left, Vector256.LoadUnsafe(ref source, (nuint)num2));
                var vector2 = Vector256.Equals(left2, Vector256.LoadUnsafe(ref source, (nuint)(num2 + num6)));
                var vector3 = (vector & vector2).AsByte();
                if(vector3 != Vector256<byte>.Zero)
                {
                    var num7 = vector3.ExtractMostSignificantBits();
                    do
                    {
                        var num8 = 30 - BitOperations.LeadingZeroCount(num7);
                        var num9 = (int)((uint)num8 / 2u);
                        if(valueLength == 2 || SequenceEqual(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref searchSpace, num2 + num9)), ref Unsafe.As<char, byte>(ref value), (uint)valueLength * (nuint)2u))
                        {
                            return num9 + num2;
                        }

                        num7 &= (uint)~(3 << num8);
                    }
                    while(num7 != 0);
                }

                num2 -= Vector256<ushort>.Count;
                if(num2 == -Vector256<ushort>.Count)
                {
                    break;
                }

                if(num2 < 0)
                {
                    num2 = 0;
                }
            }

            return -1;
        }

        num2 = num3 - Vector128<ushort>.Count;
        var c3 = Unsafe.Add(ref value, num);
        var num10 = num;
        while(c3 == value && num10 > 1)
        {
            c3 = Unsafe.Add(ref value, --num10);
        }

        var left3 = Vector128.Create((ushort)value);
        var left4 = Vector128.Create((ushort)c3);
        while(true)
        {
            var vector4 = Vector128.Equals(left3, Vector128.LoadUnsafe(ref source, (nuint)num2));
            var vector5 = Vector128.Equals(left4, Vector128.LoadUnsafe(ref source, (nuint)(num2 + num10)));
            var vector6 = (vector4 & vector5).AsByte();
            if(vector6 != Vector128<byte>.Zero)
            {
                var num11 = vector6.ExtractMostSignificantBits();
                do
                {
                    var num12 = 30 - BitOperations.LeadingZeroCount(num11);
                    var num13 = (int)((uint)num12 / 2u);
                    if(valueLength == 2 || SequenceEqual(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref searchSpace, num2 + num13)), ref Unsafe.As<char, byte>(ref value), (uint)valueLength * (nuint)2u))
                    {
                        return num13 + num2;
                    }

                    num11 &= (uint)~(3 << num12);
                }
                while(num11 != 0);
            }

            num2 -= Vector128<ushort>.Count;
            if(num2 == -Vector128<ushort>.Count)
            {
                break;
            }

            if(num2 < 0)
            {
                num2 = 0;
            }
        }

        return -1;
    }

    public static int LastIndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
    {
        if(valueLength == 0)
        {
            return searchSpaceLength;
        }

        var num = valueLength - 1;
        if(num == 0)
        {
            return LastIndexOf(ref searchSpace, value, searchSpaceLength);
        }

        var num2 = 0;
        var value2 = value;
        ref var second = ref Unsafe.Add(ref value, 1);
        while(true)
        {
            var num3 = searchSpaceLength - num2 - num;
            if(num3 <= 0)
            {
                break;
            }

            var num4 = LastIndexOf(ref searchSpace, value2, num3);
            if(num4 < 0)
            {
                break;
            }

            if(SequenceEqual(ref Unsafe.Add(ref searchSpace, num4 + 1), ref second, num))
            {
                return num4;
            }

            num2 += num3 - num4;
        }

        return -1;
    }

    public static int LastIndexOf<T>(ref T searchSpace, T value, int length) where T : IEquatable<T>
    {
        if(default(T) != null || value != null)
        {
            while(length >= 8)
            {
                length -= 8;
                if(!value.Equals(Unsafe.Add(ref searchSpace, length + 7)))
                {
                    if(!value.Equals(Unsafe.Add(ref searchSpace, length + 6)))
                    {
                        if(!value.Equals(Unsafe.Add(ref searchSpace, length + 5)))
                        {
                            if(!value.Equals(Unsafe.Add(ref searchSpace, length + 4)))
                            {
                                if(!value.Equals(Unsafe.Add(ref searchSpace, length + 3)))
                                {
                                    if(!value.Equals(Unsafe.Add(ref searchSpace, length + 2)))
                                    {
                                        if(!value.Equals(Unsafe.Add(ref searchSpace, length + 1)))
                                        {
                                            if(!value.Equals(Unsafe.Add(ref searchSpace, length)))
                                            {
                                                continue;
                                            }

                                            goto IL_01fe;
                                        }

                                        goto IL_0200;
                                    }

                                    goto IL_0204;
                                }

                                goto IL_0208;
                            }

                            return length + 4;
                        }

                        return length + 5;
                    }

                    return length + 6;
                }

                return length + 7;
            }

            if(length >= 4)
            {
                length -= 4;
                if(value.Equals(Unsafe.Add(ref searchSpace, length + 3)))
                {
                    goto IL_0208;
                }

                if(value.Equals(Unsafe.Add(ref searchSpace, length + 2)))
                {
                    goto IL_0204;
                }

                if(value.Equals(Unsafe.Add(ref searchSpace, length + 1)))
                {
                    goto IL_0200;
                }

                if(value.Equals(Unsafe.Add(ref searchSpace, length)))
                {
                    goto IL_01fe;
                }
            }

            while(length > 0)
            {
                length--;
                if(!value.Equals(Unsafe.Add(ref searchSpace, length)))
                {
                    continue;
                }

                goto IL_01fe;
            }
        }
        else
        {
            length--;
            while(length >= 0)
            {
                if(Unsafe.Add(ref searchSpace, length) != null)
                {
                    length--;
                    continue;
                }

                goto IL_01fe;
            }
        }

        return -1;
    IL_0208:
        return length + 3;
    IL_0204:
        return length + 2;
    IL_0200:
        return length + 1;
    IL_01fe:
        return length;
    }

    public static int LastIndexOfAny<T>(ref T searchSpace, T value0, T value1, int length) where T : IEquatable<T>
    {
        if(default(T) != null || value0 != null && value1 != null)
        {
            while(length >= 8)
            {
                length -= 8;
                var other = Unsafe.Add(ref searchSpace, length + 7);
                if(!value0.Equals(other) && !value1.Equals(other))
                {
                    other = Unsafe.Add(ref searchSpace, length + 6);
                    if(!value0.Equals(other) && !value1.Equals(other))
                    {
                        other = Unsafe.Add(ref searchSpace, length + 5);
                        if(!value0.Equals(other) && !value1.Equals(other))
                        {
                            other = Unsafe.Add(ref searchSpace, length + 4);
                            if(!value0.Equals(other) && !value1.Equals(other))
                            {
                                other = Unsafe.Add(ref searchSpace, length + 3);
                                if(!value0.Equals(other) && !value1.Equals(other))
                                {
                                    other = Unsafe.Add(ref searchSpace, length + 2);
                                    if(!value0.Equals(other) && !value1.Equals(other))
                                    {
                                        other = Unsafe.Add(ref searchSpace, length + 1);
                                        if(!value0.Equals(other) && !value1.Equals(other))
                                        {
                                            other = Unsafe.Add(ref searchSpace, length);
                                            if(!value0.Equals(other) && !value1.Equals(other))
                                            {
                                                continue;
                                            }

                                            goto IL_0351;
                                        }

                                        goto IL_0353;
                                    }

                                    goto IL_0357;
                                }

                                goto IL_035b;
                            }

                            return length + 4;
                        }

                        return length + 5;
                    }

                    return length + 6;
                }

                return length + 7;
            }

            if(length >= 4)
            {
                length -= 4;
                var other = Unsafe.Add(ref searchSpace, length + 3);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_035b;
                }

                other = Unsafe.Add(ref searchSpace, length + 2);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_0357;
                }

                other = Unsafe.Add(ref searchSpace, length + 1);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_0353;
                }

                other = Unsafe.Add(ref searchSpace, length);
                if(value0.Equals(other) || value1.Equals(other))
                {
                    goto IL_0351;
                }
            }

            while(length > 0)
            {
                length--;
                var other = Unsafe.Add(ref searchSpace, length);
                if(!value0.Equals(other) && !value1.Equals(other))
                {
                    continue;
                }

                goto IL_0351;
            }
        }
        else
        {
            for(length--; length >= 0; length--)
            {
                var other = Unsafe.Add(ref searchSpace, length);
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

                goto IL_0351;
            }
        }

        return -1;
    IL_0357:
        return length + 2;
    IL_0351:
        return length;
    IL_035b:
        return length + 3;
    IL_0353:
        return length + 1;
    }

    public static int LastIndexOfAny<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : IEquatable<T>
    {
        if(default(T) != null || value0 != null && value1 != null && value2 != null)
        {
            while(length >= 8)
            {
                length -= 8;
                var other = Unsafe.Add(ref searchSpace, length + 7);
                if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                {
                    other = Unsafe.Add(ref searchSpace, length + 6);
                    if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                    {
                        other = Unsafe.Add(ref searchSpace, length + 5);
                        if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                        {
                            other = Unsafe.Add(ref searchSpace, length + 4);
                            if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                            {
                                other = Unsafe.Add(ref searchSpace, length + 3);
                                if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                                {
                                    other = Unsafe.Add(ref searchSpace, length + 2);
                                    if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                                    {
                                        other = Unsafe.Add(ref searchSpace, length + 1);
                                        if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                                        {
                                            other = Unsafe.Add(ref searchSpace, length);
                                            if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                                            {
                                                continue;
                                            }

                                            goto IL_0485;
                                        }

                                        goto IL_0488;
                                    }

                                    goto IL_048d;
                                }

                                goto IL_0492;
                            }

                            return length + 4;
                        }

                        return length + 5;
                    }

                    return length + 6;
                }

                return length + 7;
            }

            if(length >= 4)
            {
                length -= 4;
                var other = Unsafe.Add(ref searchSpace, length + 3);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_0492;
                }

                other = Unsafe.Add(ref searchSpace, length + 2);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_048d;
                }

                other = Unsafe.Add(ref searchSpace, length + 1);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_0488;
                }

                other = Unsafe.Add(ref searchSpace, length);
                if(value0.Equals(other) || value1.Equals(other) || value2.Equals(other))
                {
                    goto IL_0485;
                }
            }

            while(length > 0)
            {
                length--;
                var other = Unsafe.Add(ref searchSpace, length);
                if(!value0.Equals(other) && !value1.Equals(other) && !value2.Equals(other))
                {
                    continue;
                }

                goto IL_0485;
            }
        }
        else
        {
            for(length--; length >= 0; length--)
            {
                var other = Unsafe.Add(ref searchSpace, length);
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

                goto IL_0485;
            }
        }

        return -1;
    IL_0488:
        return length + 1;
    IL_0492:
        return length + 3;
    IL_0485:
        return length;
    IL_048d:
        return length + 2;
    }

    public static int LastIndexOfAny<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
    {
        if(valueLength == 0)
        {
            return -1;
        }

        if(typeof(T).IsValueType)
        {
            for(var num = searchSpaceLength - 1; num >= 0; num--)
            {
                var other = Unsafe.Add(ref searchSpace, num);
                for(var i = 0; i < valueLength; i++)
                {
                    if(Unsafe.Add(ref value, i).Equals(other))
                    {
                        return num;
                    }
                }
            }
        }
        else
        {
            for(var num2 = searchSpaceLength - 1; num2 >= 0; num2--)
            {
                var val = Unsafe.Add(ref searchSpace, num2);
                if(val != null)
                {
                    for(var j = 0; j < valueLength; j++)
                    {
                        if(val.Equals(Unsafe.Add(ref value, j)))
                        {
                            return num2;
                        }
                    }
                }
                else
                {
                    for(var k = 0; k < valueLength; k++)
                    {
                        if(Unsafe.Add(ref value, k) == null)
                        {
                            return num2;
                        }
                    }
                }
            }
        }

        return -1;
    }

    internal static int LastIndexOfAnyExcept<T>(ref T searchSpace, T value0, int length)
    {
        for(var num = length - 1; num >= 0; num--)
        {
            if(!EqualityComparer<T>.Default.Equals(Unsafe.Add(ref searchSpace, num), value0))
            {
                return num;
            }
        }

        return -1;
    }

    internal static int LastIndexOfAnyExcept<T>(ref T searchSpace, T value0, T value1, int length)
    {
        for(var num = length - 1; num >= 0; num--)
        {
            ref var reference = ref Unsafe.Add(ref searchSpace, num);
            if(!EqualityComparer<T>.Default.Equals(reference, value0) && !EqualityComparer<T>.Default.Equals(reference, value1))
            {
                return num;
            }
        }

        return -1;
    }

    internal static int LastIndexOfAnyExcept<T>(ref T searchSpace, T value0, T value1, T value2, int length)
    {
        for(var num = length - 1; num >= 0; num--)
        {
            ref var reference = ref Unsafe.Add(ref searchSpace, num);
            if(!EqualityComparer<T>.Default.Equals(reference, value0) && !EqualityComparer<T>.Default.Equals(reference, value1) && !EqualityComparer<T>.Default.Equals(reference, value2))
            {
                return num;
            }
        }

        return -1;
    }

    internal static int LastIndexOfAnyExcept<T>(ref T searchSpace, T value0, T value1, T value2, T value3, int length)
    {
        for(var num = length - 1; num >= 0; num--)
        {
            ref var reference = ref Unsafe.Add(ref searchSpace, num);
            if(!EqualityComparer<T>.Default.Equals(reference, value0) && !EqualityComparer<T>.Default.Equals(reference, value1) && !EqualityComparer<T>.Default.Equals(reference, value2) && !EqualityComparer<T>.Default.Equals(reference, value3))
            {
                return num;
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfAnyExceptValueType<T>(ref T searchSpace, T value, int length) where T : struct, INumber<T>
    {
        return LastIndexOfValueType<T, Negate<T>>(ref searchSpace, value, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfAnyExceptValueType<T>(ref T searchSpace, T value0, T value1, int length) where T : struct, INumber<T>
    {
        return LastIndexOfAnyValueType<T, Negate<T>>(ref searchSpace, value0, value1, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfAnyExceptValueType<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : struct, INumber<T>
    {
        return LastIndexOfAnyValueType<T, Negate<T>>(ref searchSpace, value0, value1, value2, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfAnyExceptValueType<T>(ref T searchSpace, T value0, T value1, T value2, T value3, int length) where T : struct, INumber<T>
    {
        return LastIndexOfAnyValueType<T, Negate<T>>(ref searchSpace, value0, value1, value2, value3, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfAnyExceptValueType<T>(ref T searchSpace, T value0, T value1, T value2, T value3, T value4, int length) where T : struct, INumber<T>
           => LastIndexOfAnyValueType<T, Negate<T>>(ref searchSpace, value0, value1, value2, value3, value4, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfAnyValueType<T>(ref T searchSpace, T value0, T value1, int length) where T : struct, INumber<T>
    {
        return LastIndexOfAnyValueType<T, DontNegate<T>>(ref searchSpace, value0, value1, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfAnyValueType<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : struct, INumber<T>
    {
        return LastIndexOfAnyValueType<T, DontNegate<T>>(ref searchSpace, value0, value1, value2, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfAnyValueType<T>(ref T searchSpace, T value0, T value1, T value2, T value3, int length) where T : struct, INumber<T>
    {
        return LastIndexOfAnyValueType<T, DontNegate<T>>(ref searchSpace, value0, value1, value2, value3, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfAnyValueType<T>(ref T searchSpace, T value0, T value1, T value2, T value3, T value4, int length) where T : struct, INumber<T>
            => LastIndexOfAnyValueType<T, DontNegate<T>>(ref searchSpace, value0, value1, value2, value3, value4, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LastIndexOfValueType<T>(ref T searchSpace, T value, int length) where T : struct, INumber<T>
    {
        return LastIndexOfValueType<T, DontNegate<T>>(ref searchSpace, value, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int LastIndexOfValueType<TValue, TNegator>(
        ref TValue searchSpace,
        TValue value,
        int length) where TValue : struct, INumber<TValue> where TNegator : struct, INegator<TValue>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            var num = (nuint)length - 1u;
            while(length >= 8)
            {
                length -= 8;
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num) == value))
                {
                    return (int)num;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 1) == value))
                {
                    return (int)num - 1;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 2) == value))
                {
                    return (int)num - 2;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 3) == value))
                {
                    return (int)num - 3;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 4) == value))
                {
                    return (int)num - 4;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 5) == value))
                {
                    return (int)num - 5;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 6) == value))
                {
                    return (int)num - 6;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 7) == value))
                {
                    return (int)num - 7;
                }

                num -= 8;
            }

            if(length >= 4)
            {
                length -= 4;
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num) == value))
                {
                    return (int)num;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 1) == value))
                {
                    return (int)num - 1;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 2) == value))
                {
                    return (int)num - 2;
                }

                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num - 3) == value))
                {
                    return (int)num - 3;
                }

                num -= 4;
            }

            while(length > 0)
            {
                length--;
                if(TNegator.NegateIfNeeded(Unsafe.Add(ref searchSpace, num) == value))
                {
                    return (int)num;
                }

                num--;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            var left = Vector256.Create(value);
            nint num2 = length - Vector256<TValue>.Count;
            Vector256<TValue> vector;
            while(num2 > 0)
            {
                vector = TNegator.NegateIfNeeded(Vector256.Equals(left, Vector256.LoadUnsafe(ref searchSpace, (nuint)num2)));
                if(vector == Vector256<TValue>.Zero)
                {
                    num2 -= Vector256<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(num2, vector);
            }

            vector = TNegator.NegateIfNeeded(Vector256.Equals(left, Vector256.LoadUnsafe(ref searchSpace)));
            if(vector != Vector256<TValue>.Zero)
            {
                return ComputeLastIndex(0, vector);
            }
        }
        else
        {
            var left2 = Vector128.Create(value);
            nint num3 = length - Vector128<TValue>.Count;
            Vector128<TValue> vector2;
            while(num3 > 0)
            {
                vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, Vector128.LoadUnsafe(ref searchSpace, (nuint)num3)));
                if(vector2 == Vector128<TValue>.Zero)
                {
                    num3 -= Vector128<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(num3, vector2);
            }

            vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, Vector128.LoadUnsafe(ref searchSpace)));
            if(vector2 != Vector128<TValue>.Zero)
            {
                return ComputeLastIndex(0, vector2);
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int LastIndexOfAnyValueType<TValue, TNegator>(
        ref TValue searchSpace,
        TValue value0,
        TValue value1,
        int length)
        where TValue : struct, INumber<TValue>
        where TNegator : struct, INegator<TValue>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            var num = (nuint)length - 1u;
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

                    val = Unsafe.Add(ref reference, -1);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num - 1;
                    }

                    val = Unsafe.Add(ref reference, -2);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num - 2;
                    }

                    val = Unsafe.Add(ref reference, -3);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num - 3;
                    }

                    val = Unsafe.Add(ref reference, -4);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num - 4;
                    }

                    val = Unsafe.Add(ref reference, -5);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num - 5;
                    }

                    val = Unsafe.Add(ref reference, -6);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num - 6;
                    }

                    val = Unsafe.Add(ref reference, -7);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                    {
                        return (int)num - 7;
                    }

                    num -= 8;
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

                val = Unsafe.Add(ref reference2, -1);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                {
                    return (int)num - 1;
                }

                val = Unsafe.Add(ref reference2, -2);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                {
                    return (int)num - 2;
                }

                val = Unsafe.Add(ref reference2, -3);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                {
                    return (int)num - 3;
                }

                num -= 4;
            }

            while(length > 0)
            {
                length--;
                var val = Unsafe.Add(ref searchSpace, num);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1))
                {
                    return (int)num;
                }

                num--;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            var right = Vector256.Create(value0);
            var right2 = Vector256.Create(value1);
            nint num2 = length - Vector256<TValue>.Count;
            Vector256<TValue> left;
            Vector256<TValue> vector;
            while(num2 > 0)
            {
                left = Vector256.LoadUnsafe(ref searchSpace, (nuint)num2);
                vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left, right2));
                if(vector == Vector256<TValue>.Zero)
                {
                    num2 -= Vector256<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(num2, vector);
            }

            left = Vector256.LoadUnsafe(ref searchSpace);
            vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left, right2));
            if(vector != Vector256<TValue>.Zero)
            {
                return ComputeLastIndex(0, vector);
            }
        }
        else
        {
            var right3 = Vector128.Create(value0);
            var right4 = Vector128.Create(value1);
            nint num3 = length - Vector128<TValue>.Count;
            Vector128<TValue> left2;
            Vector128<TValue> vector2;
            while(num3 > 0)
            {
                left2 = Vector128.LoadUnsafe(ref searchSpace, (nuint)num3);
                vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, right3) | Vector128.Equals(left2, right4));
                if(vector2 == Vector128<TValue>.Zero)
                {
                    num3 -= Vector128<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(num3, vector2);
            }

            left2 = Vector128.LoadUnsafe(ref searchSpace);
            vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, right3) | Vector128.Equals(left2, right4));
            if(vector2 != Vector128<TValue>.Zero)
            {
                return ComputeLastIndex(0, vector2);
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int LastIndexOfAnyValueType<TValue, TNegator>(
        ref TValue searchSpace,
        TValue value0,
        TValue value1,
        TValue value2,
        int length) where TValue : struct, INumber<TValue> where TNegator : struct, INegator<TValue>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            var num = (nuint)length - 1u;
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

                    val = Unsafe.Add(ref reference, -1);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num - 1;
                    }

                    val = Unsafe.Add(ref reference, -2);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num - 2;
                    }

                    val = Unsafe.Add(ref reference, -3);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num - 3;
                    }

                    val = Unsafe.Add(ref reference, -4);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num - 4;
                    }

                    val = Unsafe.Add(ref reference, -5);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num - 5;
                    }

                    val = Unsafe.Add(ref reference, -6);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num - 6;
                    }

                    val = Unsafe.Add(ref reference, -7);
                    if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                    {
                        return (int)num - 7;
                    }

                    num -= 8;
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

                val = Unsafe.Add(ref reference2, -1);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                {
                    return (int)num - 1;
                }

                val = Unsafe.Add(ref reference2, -2);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                {
                    return (int)num - 2;
                }

                val = Unsafe.Add(ref reference2, -3);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                {
                    return (int)num - 3;
                }

                num -= 4;
            }

            while(length > 0)
            {
                length--;
                var val = Unsafe.Add(ref searchSpace, num);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2))
                {
                    return (int)num;
                }

                num--;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            var right = Vector256.Create(value0);
            var right2 = Vector256.Create(value1);
            var right3 = Vector256.Create(value2);
            nint num2 = length - Vector256<TValue>.Count;
            Vector256<TValue> left;
            Vector256<TValue> vector;
            while(num2 > 0)
            {
                left = Vector256.LoadUnsafe(ref searchSpace, (nuint)num2);
                vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left, right2) | Vector256.Equals(left, right3));
                if(vector == Vector256<TValue>.Zero)
                {
                    num2 -= Vector256<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(num2, vector);
            }

            left = Vector256.LoadUnsafe(ref searchSpace);
            vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left, right2) | Vector256.Equals(left, right3));
            if(vector != Vector256<TValue>.Zero)
            {
                return ComputeLastIndex(0, vector);
            }
        }
        else
        {
            var right4 = Vector128.Create(value0);
            var right5 = Vector128.Create(value1);
            var right6 = Vector128.Create(value2);
            nint num3 = length - Vector128<TValue>.Count;
            Vector128<TValue> left2;
            Vector128<TValue> vector2;
            while(num3 > 0)
            {
                left2 = Vector128.LoadUnsafe(ref searchSpace, (nuint)num3);
                vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, right4) | Vector128.Equals(left2, right5) | Vector128.Equals(left2, right6));
                if(vector2 == Vector128<TValue>.Zero)
                {
                    num3 -= Vector128<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(num3, vector2);
            }

            left2 = Vector128.LoadUnsafe(ref searchSpace);
            vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, right4) | Vector128.Equals(left2, right5) | Vector128.Equals(left2, right6));
            if(vector2 != Vector128<TValue>.Zero)
            {
                return ComputeLastIndex(0, vector2);
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int LastIndexOfAnyValueType<TValue, TNegator>(
        ref TValue searchSpace,
        TValue value0,
        TValue value1,
        TValue value2,
        TValue value3,
        int length) where TValue : struct, INumber<TValue> where TNegator : struct, INegator<TValue>
    {
        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            var num = (nuint)length - 1u;
            while(length >= 4)
            {
                length -= 4;
                ref var reference = ref Unsafe.Add(ref searchSpace, num);
                var val = reference;
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num;
                }

                val = Unsafe.Add(ref reference, -1);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num - 1;
                }

                val = Unsafe.Add(ref reference, -2);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num - 2;
                }

                val = Unsafe.Add(ref reference, -3);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num - 3;
                }

                num -= 4;
            }

            while(length > 0)
            {
                length--;
                var val = Unsafe.Add(ref searchSpace, num);
                if(TNegator.NegateIfNeeded(val == value0 || val == value1 || val == value2 || val == value3))
                {
                    return (int)num;
                }

                num--;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            var right = Vector256.Create(value0);
            var right2 = Vector256.Create(value1);
            var right3 = Vector256.Create(value2);
            var right4 = Vector256.Create(value3);
            nint num2 = length - Vector256<TValue>.Count;
            Vector256<TValue> left;
            Vector256<TValue> vector;
            while(num2 > 0)
            {
                left = Vector256.LoadUnsafe(ref searchSpace, (nuint)num2);
                vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left, right2) | Vector256.Equals(left, right3) | Vector256.Equals(left, right4));
                if(vector == Vector256<TValue>.Zero)
                {
                    num2 -= Vector256<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(num2, vector);
            }

            left = Vector256.LoadUnsafe(ref searchSpace);
            vector = TNegator.NegateIfNeeded(Vector256.Equals(left, right) | Vector256.Equals(left, right2) | Vector256.Equals(left, right3) | Vector256.Equals(left, right4));
            if(vector != Vector256<TValue>.Zero)
            {
                return ComputeLastIndex(0, vector);
            }
        }
        else
        {
            var right5 = Vector128.Create(value0);
            var right6 = Vector128.Create(value1);
            var right7 = Vector128.Create(value2);
            var right8 = Vector128.Create(value3);
            nint num3 = length - Vector128<TValue>.Count;
            Vector128<TValue> left2;
            Vector128<TValue> vector2;
            while(num3 > 0)
            {
                left2 = Vector128.LoadUnsafe(ref searchSpace, (nuint)num3);
                vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, right5) | Vector128.Equals(left2, right6) | Vector128.Equals(left2, right7) | Vector128.Equals(left2, right8));
                if(vector2 == Vector128<TValue>.Zero)
                {
                    num3 -= Vector128<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(num3, vector2);
            }

            left2 = Vector128.LoadUnsafe(ref searchSpace);
            vector2 = TNegator.NegateIfNeeded(Vector128.Equals(left2, right5) | Vector128.Equals(left2, right6) | Vector128.Equals(left2, right7) | Vector128.Equals(left2, right8));
            if(vector2 != Vector128<TValue>.Zero)
            {
                return ComputeLastIndex(0, vector2);
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int LastIndexOfAnyValueType<TValue, TNegator>(
        ref TValue searchSpace,
        TValue value0,
        TValue value1,
        TValue value2,
        TValue value3,
        TValue value4,
        int length)
            where TValue : struct, INumber<TValue>
            where TNegator : struct, INegator<TValue>
    {
        Debug.Assert(length >= 0, "Expected non-negative length");
        Debug.Assert(value0 is byte or short or int or long, "Expected caller to normalize to one of these types");

        if(!Vector128.IsHardwareAccelerated || length < Vector128<TValue>.Count)
        {
            nuint offset = (nuint)length - 1;
            TValue lookUp;

            while(length >= 4)
            {
                length -= 4;

                ref TValue current = ref Unsafe.Add(ref searchSpace, offset);
                lookUp = current;
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    return (int)offset;
                lookUp = Unsafe.Add(ref current, -1);
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    return (int)offset - 1;
                lookUp = Unsafe.Add(ref current, -2);
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    return (int)offset - 2;
                lookUp = Unsafe.Add(ref current, -3);
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    return (int)offset - 3;

                offset -= 4;
            }

            while(length > 0)
            {
                length -= 1;

                lookUp = Unsafe.Add(ref searchSpace, offset);
                if(TNegator.NegateIfNeeded(lookUp == value0 || lookUp == value1 || lookUp == value2 || lookUp == value3 || lookUp == value4))
                    return (int)offset;

                offset -= 1;
            }
        }
        else if(Vector256.IsHardwareAccelerated && length >= Vector256<TValue>.Count)
        {
            Vector256<TValue> equals, current, values0 = Vector256.Create(value0), values1 = Vector256.Create(value1),
                values2 = Vector256.Create(value2), values3 = Vector256.Create(value3), values4 = Vector256.Create(value4);
            nint offset = length - Vector256<TValue>.Count;

            // Loop until either we've finished all elements or there's less than a vector's-worth remaining.
            while(offset > 0)
            {
                current = Vector256.LoadUnsafe(ref searchSpace, (nuint)(offset));
                equals = TNegator.NegateIfNeeded(Vector256.Equals(current, values0) | Vector256.Equals(current, values1) | Vector256.Equals(current, values2)
                    | Vector256.Equals(current, values3) | Vector256.Equals(current, values4));
                if(equals == Vector256<TValue>.Zero)
                {
                    offset -= Vector256<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(offset, equals);
            }

            // Process the first vector in the search space.

            current = Vector256.LoadUnsafe(ref searchSpace);
            equals = TNegator.NegateIfNeeded(Vector256.Equals(current, values0) | Vector256.Equals(current, values1) | Vector256.Equals(current, values2)
                | Vector256.Equals(current, values3) | Vector256.Equals(current, values4));

            if(equals != Vector256<TValue>.Zero)
            {
                return ComputeLastIndex(offset: 0, equals);
            }
        }
        else
        {
            Vector128<TValue> equals, current, values0 = Vector128.Create(value0), values1 = Vector128.Create(value1),
                values2 = Vector128.Create(value2), values3 = Vector128.Create(value3), values4 = Vector128.Create(value4);
            nint offset = length - Vector128<TValue>.Count;

            // Loop until either we've finished all elements or there's less than a vector's-worth remaining.
            while(offset > 0)
            {
                current = Vector128.LoadUnsafe(ref searchSpace, (nuint)(offset));
                equals = TNegator.NegateIfNeeded(Vector128.Equals(current, values0) | Vector128.Equals(current, values1) | Vector128.Equals(current, values2)
                    | Vector128.Equals(current, values3) | Vector128.Equals(current, values4));

                if(equals == Vector128<TValue>.Zero)
                {
                    offset -= Vector128<TValue>.Count;
                    continue;
                }

                return ComputeLastIndex(offset, equals);
            }

            // Process the first vector in the search space.

            current = Vector128.LoadUnsafe(ref searchSpace);
            equals = TNegator.NegateIfNeeded(Vector128.Equals(current, values0) | Vector128.Equals(current, values1) | Vector128.Equals(current, values2)
                | Vector128.Equals(current, values3) | Vector128.Equals(current, values4));

            if(equals != Vector128<TValue>.Zero)
            {
                return ComputeLastIndex(offset: 0, equals);
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ComputeLastIndex<T>(nint offset, Vector128<T> equals) where T : struct
    {
        var value = equals.ExtractMostSignificantBits();
        var num = 31 - BitOperations.LeadingZeroCount(value);
        return (int)offset + num;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ComputeLastIndex<T>(nint offset, Vector256<T> equals) where T : struct
    {
        var value = equals.ExtractMostSignificantBits();
        var num = 31 - BitOperations.LeadingZeroCount(value);
        return (int)offset + num;
    }
}
