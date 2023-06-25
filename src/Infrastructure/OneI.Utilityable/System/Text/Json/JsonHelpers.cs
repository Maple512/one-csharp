namespace System.Text.Json;

using System.Buffers;
using System.Text.Encodings.Web;
using Cysharp.Text;

/// <summary>
/// <see href="https://github.com/dotnet/runtime/blob/06bf1c6f6e54994bfecbadf5bc318d97dcd711cc/src/libraries/System.Text.Json/src/System/Text/Json/JsonHelpers.Escaping.cs#L28"/>
/// </summary>
public static class JsonHelpers
{
    // Only allow ASCII characters between ' ' (0x20) and '~' (0x7E), inclusively,
    // but exclude characters that need to be escaped as hex: '"', '\'', '&', '+', '<', '>', '`'
    // and exclude characters that need to be escaped by adding a backslash: '\n', '\r', '\t', '\\', '\b', '\f'
    //
    // non-zero = allowed, 0 = disallowed
    public const int LastAsciiCharacter = 0x7F;

    private static readonly StandardFormat s_hexStandardFormat = new('X', 4);

    public static string Escape(ReadOnlySpan<byte> chars, JavaScriptEncoder? encoder = null)
    {
        if(chars.IsEmpty)
        {
            return string.Empty;
        }

        // 2k
        if(chars.Length > 2 * 1024)
        {
            throw new ArgumentOutOfRangeException(nameof(chars));
        }

        encoder ??= JavaScriptEncoder.Default;

        using var container = ZString.CreateUtf8StringBuilder(true);

        scoped var span = container.GetSpan(0);

        var index = encoder.FindFirstCharacterToEncodeUtf8(chars);

        EscapeString(chars, span, index, encoder, out var written);

        container.Advance(written);

        return container.ToString();
    }

    private static void EscapeString(
        ReadOnlySpan<byte> value,
        Span<byte> destination,
        int indexOfFirstByteToEscape,
        JavaScriptEncoder encoder,
        out int written)
    {
        Debug.Assert(indexOfFirstByteToEscape >= 0 && indexOfFirstByteToEscape < value.Length);

        value[..indexOfFirstByteToEscape].CopyTo(destination);

        written = indexOfFirstByteToEscape;

        destination = destination[indexOfFirstByteToEscape..];
        value = value[indexOfFirstByteToEscape..];

        EscapeStringCore(value, destination, encoder, ref written);
    }

    private static void EscapeStringCore(ReadOnlySpan<byte> value, Span<byte> destination, JavaScriptEncoder encoder, ref int written)
    {
        Debug.Assert(encoder != null);

        var result = encoder.EncodeUtf8(value, destination, out var encoderBytesConsumed, out var encoderBytesWritten);

        Debug.Assert(result != OperationStatus.DestinationTooSmall);
        Debug.Assert(result != OperationStatus.NeedMoreData);

        if(result != OperationStatus.Done)
        {
            ThrowArgumentException_InvalidUTF8(value[encoderBytesWritten..]);
        }

        Debug.Assert(encoderBytesConsumed == value.Length);

        written += encoderBytesWritten;
    }

    [DoesNotReturn]
    private static void ThrowArgumentException_InvalidUTF8(ReadOnlySpan<byte> value)
    {
        var builder = new StringBuilder();

        var printFirst10 = Math.Min(value.Length, 10);

        for(var i = 0; i < printFirst10; i++)
        {
            var nextByte = value[i];
            if(IsPrintable(nextByte))
            {
                _ = builder.Append((char)nextByte);
            }
            else
            {
                _ = builder.Append($"0x{nextByte:X2}");
            }
        }

        if(printFirst10 < value.Length)
        {
            _ = builder.Append("...");
        }

        throw new ArgumentException($"Cannot encode invalid UTF-8 text as JSON. Invalid input: '{builder}'.");
    }

    private static bool IsPrintable(byte value) => value is >= 0x20 and < 0x7F;
}
