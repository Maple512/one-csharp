namespace OneI.Buffers;

using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

internal static partial class ValueHelper
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int SequenceCompareTo(ref byte first, int firstLength, ref byte second, int secondLength)
    {
        nuint num;
        uint num6;
        nuint num3;
        nuint num2;
        if(!Unsafe.AreSame(ref first, ref second))
        {
            num = (uint)(((uint)firstLength < (uint)secondLength) ? firstLength : secondLength);
            num2 = 0u;
            num3 = num;
            if(!Avx2.IsSupported)
            {
                if(Sse2.IsSupported)
                {
                    if(num3 >= (nuint)Vector128<byte>.Count)
                    {
                        num3 -= (nuint)Vector128<byte>.Count;
                        while(true)
                        {
                            uint num4;
                            if(num3 > num2)
                            {
                                num4 = (uint)Sse2.MoveMask(Sse2.CompareEqual(LoadVector128(ref first, num2), LoadVector128(ref second, num2)));
                                if(num4 == 65535)
                                {
                                    num2 += (nuint)Vector128<byte>.Count;
                                    continue;
                                }
                            }
                            else
                            {
                                num2 = num3;
                                num4 = (uint)Sse2.MoveMask(Sse2.CompareEqual(LoadVector128(ref first, num2), LoadVector128(ref second, num2)));
                                if(num4 == 65535)
                                {
                                    break;
                                }
                            }

                            var value = ~num4;
                            num2 += (uint)BitOperations.TrailingZeroCount(value);
                            return Unsafe.AddByteOffset(ref first, num2).CompareTo(Unsafe.AddByteOffset(ref second, num2));
                        }

                        goto IL_0274;
                    }
                }
                else if(Vector.IsHardwareAccelerated && num3 > (nuint)Vector<byte>.Count)
                {
                    for(num3 -= (nuint)Vector<byte>.Count; num3 > num2 && !(LoadVector(ref first, num2) != LoadVector(ref second, num2)); num2 += (nuint)Vector<byte>.Count)
                    {
                    }

                    goto IL_0270;
                }

                goto IL_021b;
            }

            if(num3 >= (nuint)Vector256<byte>.Count)
            {
                num3 -= (nuint)Vector256<byte>.Count;
                while(true)
                {
                    uint num5;
                    if(num3 > num2)
                    {
                        num5 = (uint)Avx2.MoveMask(Avx2.CompareEqual(LoadVector256(ref first, num2), LoadVector256(ref second, num2)));
                        if(num5 == uint.MaxValue)
                        {
                            num2 += (nuint)Vector256<byte>.Count;
                            continue;
                        }
                    }
                    else
                    {
                        num2 = num3;
                        num5 = (uint)Avx2.MoveMask(Avx2.CompareEqual(LoadVector256(ref first, num2), LoadVector256(ref second, num2)));
                        if(num5 == uint.MaxValue)
                        {
                            break;
                        }
                    }

                    var value2 = ~num5;
                    num2 += (uint)BitOperations.TrailingZeroCount(value2);
                    return Unsafe.AddByteOffset(ref first, num2).CompareTo(Unsafe.AddByteOffset(ref second, num2));
                }
            }
            else
            {
                if(num3 < (nuint)Vector128<byte>.Count)
                {
                    goto IL_021b;
                }

                num3 -= (nuint)Vector128<byte>.Count;
                if(num3 > num2)
                {
                    num6 = (uint)Sse2.MoveMask(Sse2.CompareEqual(LoadVector128(ref first, num2), LoadVector128(ref second, num2)));
                    if(num6 != 65535)
                    {
                        goto IL_0111;
                    }
                }

                num2 = num3;
                num6 = (uint)Sse2.MoveMask(Sse2.CompareEqual(LoadVector128(ref first, num2), LoadVector128(ref second, num2)));
                if(num6 != 65535)
                {
                    goto IL_0111;
                }
            }
        }

        goto IL_0274;
    IL_021b:
        if(num3 > 8)
        {
            for(num3 -= 8; num3 > num2 && LoadNUInt(ref first, num2) == LoadNUInt(ref second, num2); num2 += 8)
            {
            }
        }

        goto IL_0270;
    IL_0111:
        var value3 = ~num6;
        num2 += (uint)BitOperations.TrailingZeroCount(value3);
        return Unsafe.AddByteOffset(ref first, num2).CompareTo(Unsafe.AddByteOffset(ref second, num2));
    IL_0270:
        for(; num > num2; num2++)
        {
            var num7 = Unsafe.AddByteOffset(ref first, num2).CompareTo(Unsafe.AddByteOffset(ref second, num2));
            if(num7 != 0)
            {
                return num7;
            }
        }

        goto IL_0274;
    IL_0274:
        return firstLength - secondLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int SequenceCompareTo(ref char first, int firstLength, ref char second, int secondLength)
    {
        var result = firstLength - secondLength;
        if(!Unsafe.AreSame(ref first, ref second))
        {
            nuint num = (uint)(((uint)firstLength < (uint)secondLength) ? firstLength : secondLength);
            nuint num2 = 0u;
            if(num >= 8 / 2)
            {
                if(Vector.IsHardwareAccelerated && num >= (nuint)Vector<ushort>.Count)
                {
                    var num3 = num - (nuint)Vector<ushort>.Count;
                    while(!(Unsafe.ReadUnaligned<Vector<ushort>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref first, (nint)num2))) != Unsafe.ReadUnaligned<Vector<ushort>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref second, (nint)num2)))))
                    {
                        num2 += (nuint)Vector<ushort>.Count;
                        if(num3 < num2)
                        {
                            break;
                        }
                    }
                }

                for(; num >= num2 + 8 / 2 && Unsafe.ReadUnaligned<nuint>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref first, (nint)num2))) == Unsafe.ReadUnaligned<nuint>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref second, (nint)num2))); num2 += 8 / 2)
                {
                }
            }

            if(num >= num2 + 2 && Unsafe.ReadUnaligned<int>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref first, (nint)num2))) == Unsafe.ReadUnaligned<int>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref second, (nint)num2))))
            {
                num2 += 2;
            }

            for(; num2 < num; num2++)
            {
                var num4 = Unsafe.Add(ref first, (nint)num2).CompareTo(Unsafe.Add(ref second, (nint)num2));
                if(num4 != 0)
                {
                    return num4;
                }
            }
        }

        return result;
    }

    public static int SequenceCompareTo<T>(ref T first, int firstLength, ref T second, int secondLength) where T : IComparable<T>
    {
        var num = firstLength;
        if(num > secondLength)
        {
            num = secondLength;
        }

        for(var i = 0; i < num; i++)
        {
            var val = Unsafe.Add(ref second, i);
            var num2 = Unsafe.Add(ref first, i)?.CompareTo(val) ?? ((val != null) ? (-1) : 0);
            if(num2 != 0)
            {
                return num2;
            }
        }

        return firstLength.CompareTo(secondLength);
    }

    public static bool SequenceEqual(ref byte first, ref byte second, nuint length)
    {
        nuint num2;
        nuint num4;
        nuint num6;
        nuint num9;
        nuint num11;
        switch((nint)length)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            {
                var num7 = 0u;
                var num8 = (nuint)((nint)length & 2);
                if(num8 != 0)
                {
                    num7 = LoadUShort(ref first);
                    num7 -= LoadUShort(ref second);
                }

                if(((nint)length & 1) != 0)
                {
                    num7 |= (uint)(Unsafe.AddByteOffset(ref first, num8) - Unsafe.AddByteOffset(ref second, num8));
                }

                return num7 == 0;
            }
            case 4:
            case 5:
            case 6:
            case 7:
            {
                var offset2 = (nuint)((nint)length - 4);
                var num12 = LoadUInt(ref first) - LoadUInt(ref second);
                num12 |= LoadUInt(ref first, offset2) - LoadUInt(ref second, offset2);
                return num12 == 0;
            }
            default:
            {
                if(Unsafe.AreSame(ref first, ref second))
                {
                    goto IL_0086;
                }

                if(Vector128.IsHardwareAccelerated)
                {
                    if(Vector256.IsHardwareAccelerated && length >= (nuint)Vector256<byte>.Count)
                    {
                        nuint num = 0u;
                        num2 = (nuint)((nint)length - Vector256<byte>.Count);
                        if(num2 == 0)
                        {
                            goto IL_00df;
                        }

                        while(!(Vector256.LoadUnsafe(ref first, num) != Vector256.LoadUnsafe(ref second, num)))
                        {
                            num += (nuint)Vector256<byte>.Count;
                            if(num2 > num)
                            {
                                continue;
                            }

                            goto IL_00df;
                        }
                    }
                    else
                    {
                        if(length < (nuint)Vector128<byte>.Count)
                        {
                            goto IL_01d1;
                        }

                        nuint num3 = 0u;
                        num4 = (nuint)((nint)length - Vector128<byte>.Count);
                        if(num4 == 0)
                        {
                            goto IL_0144;
                        }

                        while(!(Vector128.LoadUnsafe(ref first, num3) != Vector128.LoadUnsafe(ref second, num3)))
                        {
                            num3 += (nuint)Vector128<byte>.Count;
                            if(num4 > num3)
                            {
                                continue;
                            }

                            goto IL_0144;
                        }
                    }
                }
                else
                {
                    if(!Vector.IsHardwareAccelerated || length < (nuint)Vector<byte>.Count)
                    {
                        goto IL_01d1;
                    }

                    nuint num5 = 0u;
                    num6 = (nuint)((nint)length - Vector<byte>.Count);
                    if(num6 == 0)
                    {
                        goto IL_01b2;
                    }

                    while(!(LoadVector(ref first, num5) != LoadVector(ref second, num5)))
                    {
                        num5 += (nuint)Vector<byte>.Count;
                        if(num6 > num5)
                        {
                            continue;
                        }

                        goto IL_01b2;
                    }
                }

                goto IL_0262;
            }

        IL_0144:
            if(Vector128.LoadUnsafe(ref first, num4) == Vector128.LoadUnsafe(ref second, num4))
            {
                goto IL_0086;
            }

            goto IL_0262;
        IL_024a:
            return LoadNUInt(ref first, num9) == LoadNUInt(ref second, num9);
        IL_01b2:
            if(LoadVector(ref first, num6) == LoadVector(ref second, num6))
            {
                goto IL_0086;
            }

            goto IL_0262;
        IL_01d1:
            if(Vector128.IsHardwareAccelerated)
            {
                var offset = (nuint)((nint)length - 8);
                var num10 = (nuint)((nint)LoadNUInt(ref first) - (nint)LoadNUInt(ref second));
                num10 |= (nuint)((nint)LoadNUInt(ref first, offset) - (nint)LoadNUInt(ref second, offset));
                return num10 == 0;
            }

            num11 = 0u;
            num9 = (nuint)((nint)length - 8);
            if(num9 == 0)
            {
                goto IL_024a;
            }

            while(LoadNUInt(ref first, num11) == LoadNUInt(ref second, num11))
            {
                num11 += 8;
                if(num9 > num11)
                {
                    continue;
                }

                goto IL_024a;
            }

            goto IL_0262;
        IL_00df:
            if(Vector256.LoadUnsafe(ref first, num2) == Vector256.LoadUnsafe(ref second, num2))
            {
                goto IL_0086;
            }

            goto IL_0262;
        IL_0262:
            return false;
        IL_0086:
            return true;
        }
    }

    public static bool SequenceEqual<T>(ref T first, ref T second, int length) where T : IEquatable<T>
    {
        if(Unsafe.AreSame(ref first, ref second))
        {
            goto Equal;
        }

        nint index = 0; // Use nint for arithmetic to avoid unnecessary 64->32->64 truncations
        T lookUp0;
        T lookUp1;
        while(length >= 8)
        {
            length -= 8;

            lookUp0 = Unsafe.Add(ref first, index);
            lookUp1 = Unsafe.Add(ref second, index);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 1);
            lookUp1 = Unsafe.Add(ref second, index + 1);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 2);
            lookUp1 = Unsafe.Add(ref second, index + 2);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 3);
            lookUp1 = Unsafe.Add(ref second, index + 3);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 4);
            lookUp1 = Unsafe.Add(ref second, index + 4);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 5);
            lookUp1 = Unsafe.Add(ref second, index + 5);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 6);
            lookUp1 = Unsafe.Add(ref second, index + 6);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 7);
            lookUp1 = Unsafe.Add(ref second, index + 7);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            index += 8;
        }

        if(length >= 4)
        {
            length -= 4;

            lookUp0 = Unsafe.Add(ref first, index);
            lookUp1 = Unsafe.Add(ref second, index);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 1);
            lookUp1 = Unsafe.Add(ref second, index + 1);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 2);
            lookUp1 = Unsafe.Add(ref second, index + 2);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            lookUp0 = Unsafe.Add(ref first, index + 3);
            lookUp1 = Unsafe.Add(ref second, index + 3);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            index += 4;
        }

        while(length > 0)
        {
            lookUp0 = Unsafe.Add(ref first, index);
            lookUp1 = Unsafe.Add(ref second, index);
            if(!(lookUp0?.Equals(lookUp1) ?? (object?)lookUp1 is null))
            {
                goto NotEqual;
            }

            index += 1;
            length--;
        }

    Equal:
        return true;

    NotEqual: // Workaround for https://github.com/dotnet/runtime/issues/8795
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nuint LoadNUInt(ref byte start)
    {
        return Unsafe.ReadUnaligned<nuint>(ref start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nuint LoadNUInt(ref byte start, nuint offset)
    {
        return Unsafe.ReadUnaligned<nuint>(ref Unsafe.AddByteOffset(ref start, offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LoadUInt(ref byte start)
    {
        return Unsafe.ReadUnaligned<uint>(ref start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LoadUInt(ref byte start, nuint offset)
    {
        return Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref start, offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort LoadUShort(ref byte start)
    {
        return Unsafe.ReadUnaligned<ushort>(ref start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<byte> LoadVector128(ref byte start, nuint offset)
    {
        return Unsafe.ReadUnaligned<Vector128<byte>>(ref Unsafe.AddByteOffset(ref start, offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector256<byte> LoadVector256(ref byte start, nuint offset)
    {
        return Unsafe.ReadUnaligned<Vector256<byte>>(ref Unsafe.AddByteOffset(ref start, offset));
    }
}
