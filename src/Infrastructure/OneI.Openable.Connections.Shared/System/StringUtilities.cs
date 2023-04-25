namespace System;

using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

internal static class StringUtilities
{
    /// <summary>
    /// A faster version of String.Concat(<paramref name="str"/>, <paramref name="separator"/>, <paramref name="number"/>.ToString("X8"))
    /// </summary>
    /// <remarks>source: <see href="https://github.com/dotnet/aspnetcore/blob/c4b57c452447f70ab874097436226542c70d74c0/src/Shared/ServerInfrastructure/StringUtilities.cs#L704"/></remarks>
    /// <param name="str"></param>
    /// <param name="separator"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string ConcatAsHexSuffix(string str, char separator, uint number)
    {
        var length = 1 + 8;
        if(str is not null)
        {
            length += str.Length;
        }

        return string.Create(length, (str, separator, number), PopulateSpanWithHexSuffix);
    }

    private static void PopulateSpanWithHexSuffix(Span<char> buffer, (string? str, char separator, uint number) state)
    {
        var (str, separator, number) = state;

        var i = 0;
        if(str is not null)
        {
            str.AsSpan().CopyTo(buffer);

            i = str.Length;
        }

        buffer[i] = separator;
        i++;

        if(Ssse3.IsSupported)
        {
            // The constant inline vectors are read from the data section without any additional
            // moves. See https://github.com/dotnet/runtime/issues/44115 Case 1.1 for further details.

            var lowNibbles = Ssse3.Shuffle(Vector128.CreateScalarUnsafe(number).AsByte(), Vector128.Create(
                0xF, 0xF, 3, 0xF,
                0xF, 0xF, 2, 0xF,
                0xF, 0xF, 1, 0xF,
                0xF, 0xF, 0, 0xF
            ).AsByte());

            var highNibbles = Sse2.ShiftRightLogical(Sse2.ShiftRightLogical128BitLane(lowNibbles, 2).AsInt32(), 4).AsByte();
            var indices = Sse2.And(Sse2.Or(lowNibbles, highNibbles), Vector128.Create((byte)0xF));

            // Lookup the hex values at the positions of the indices
            var hex = Ssse3.Shuffle(Vector128.Create(
                (byte)'0', (byte)'1', (byte)'2', (byte)'3',
                (byte)'4', (byte)'5', (byte)'6', (byte)'7',
                (byte)'8', (byte)'9', (byte)'A', (byte)'B',
                (byte)'C', (byte)'D', (byte)'E', (byte)'F'
            ), indices);

            // The high bytes (0x00) of the chars have also been converted to ascii hex '0', so clear them out.
            hex = Sse2.And(hex, Vector128.Create((ushort)0xFF).AsByte());

            // This generates much more efficient asm than fixing the buffer and using
            // Sse2.Store((byte*)(p + i), chars.AsByte());
            Unsafe.WriteUnaligned(
                ref Unsafe.As<char, byte>(
                    ref Unsafe.Add(ref MemoryMarshal.GetReference(buffer), i)),
                hex);
        }
        else
        {
            var index = (int)number;
            // Slice the buffer so we can use constant offsets in a backwards order
            // and the highest index [7] will eliminate the bounds checks for all the lower indicies.
            buffer = buffer[i..];

            // This must be explicity typed as ReadOnlySpan<byte>
            // This then becomes a non-allocating mapping to the data section of the assembly.
            // If it is a var, Span<byte> or byte[], it allocates the byte array per call.
            scoped var hexEncodeMap = "0123456789ABCDEF"u8;
            // Note: this only works with byte due to endian ambiguity for other types,
            // hence the later (char) casts

            buffer[7] = (char)hexEncodeMap[index & 0xF];
            buffer[6] = (char)hexEncodeMap[(index >> 4) & 0xF];
            buffer[5] = (char)hexEncodeMap[(index >> 8) & 0xF];
            buffer[4] = (char)hexEncodeMap[(index >> 12) & 0xF];
            buffer[3] = (char)hexEncodeMap[(index >> 16) & 0xF];
            buffer[2] = (char)hexEncodeMap[(index >> 20) & 0xF];
            buffer[1] = (char)hexEncodeMap[(index >> 24) & 0xF];
            buffer[0] = (char)hexEncodeMap[(index >> 28) & 0xF];
        }
    }
}
