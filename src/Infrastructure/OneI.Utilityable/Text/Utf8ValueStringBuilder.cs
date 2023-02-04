namespace OneI.Text;

using System.Buffers;
using Cysharp.Text;

public partial struct Utf8ValueStringBuilder : IDisposable, IBufferWriter<byte>
{
    public delegate bool TryFormat<T>(T value, Span<byte> destination, out int written, StandardFormat format);

    private const int ThreadStaticBufferSize = 64444;
    private const int DefaultBufferSize = 65536; // use 64K default buffer.
    public static readonly Encoding UTF8NoBom = new UTF8Encoding(false);

    [ThreadStatic]
    private static byte[]? scratchBuffer;

    [ThreadStatic]
    internal static bool scratchBufferUsed;
    private byte[] buffer;
    private int index;
    private readonly bool disposeImmediately;

    /// <summary>Length of written buffer.</summary>
    public int Length
    {
        get
        {
            return index;
        }
    }

    #region AS

    /// <summary>Get the written buffer data.</summary>
    public ReadOnlySpan<byte> AsSpan() => buffer.AsSpan(0, index);

    /// <summary>Get the written buffer data.</summary>
    public ReadOnlyMemory<byte> AsMemory() => buffer.AsMemory(0, index);

    /// <summary>Get the written buffer data.</summary>
    public ArraySegment<byte> AsArraySegment() => new(buffer, 0, index);

    #endregion

    /// <summary>
    /// Initializes a new instance
    /// </summary>
    /// <param name="disposeImmediately">
    /// If true uses thread-static buffer that is faster but must return immediately.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// This exception is thrown when <c>new StringBuilder(disposeImmediately: true)</c> or <c>ZString.CreateUtf8StringBuilder(notNested: true)</c> is nested.
    /// See the README.md
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8ValueStringBuilder(bool disposeImmediately)
    {
        if(disposeImmediately && scratchBufferUsed)
        {
            ThrowNestedException();
        }

        byte[]? buf;
        if(disposeImmediately)
        {
            buf = scratchBuffer;
            buf ??= scratchBuffer = new byte[ThreadStaticBufferSize];
            scratchBufferUsed = true;
        }
        else
        {
            buf = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);
        }

        buffer = buf;
        index = 0;
        this.disposeImmediately = disposeImmediately;
    }

    /// <summary>
    /// Return the inner buffer to pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if(buffer != null)
        {
            if(buffer.Length != ThreadStaticBufferSize)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            buffer = null!;
            index = 0;
            if(disposeImmediately)
            {
                scratchBufferUsed = false;
            }
        }
    }

    public void Clear()
    {
        index = 0;
    }

    private void TryGrow(int sizeHint)
    {
        if(buffer!.Length < index + sizeHint)
        {
            Grow(sizeHint);
        }
    }

    private void Grow(int sizeHint)
    {
        var nextSize = buffer!.Length * 2;
        if(sizeHint != 0)
        {
            nextSize = Math.Max(nextSize, index + sizeHint);
        }

        var newBuffer = ArrayPool<byte>.Shared.Rent(nextSize);

        buffer.CopyTo(newBuffer, 0);
        if(buffer.Length != ThreadStaticBufferSize)
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        buffer = newBuffer;
    }

    #region Append

    /// <summary>Appends the string representation of a specified value to this instance.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Append(char value)
    {
        var maxLen = UTF8NoBom.GetMaxByteCount(1);
        if(buffer!.Length - index < maxLen)
        {
            Grow(maxLen);
        }

        fixed(byte* bp = &buffer[index])
        {
            index += UTF8NoBom.GetBytes(&value, 1, bp, maxLen);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char value, int repeatCount)
    {
        if(repeatCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(repeatCount));
        }

        if(value <= 0x7F) // ASCII
        {
            GetSpan(repeatCount).Fill((byte)value);
            Advance(repeatCount);
        }
        else
        {
            var maxLen = UTF8NoBom.GetMaxByteCount(1);
            Span<byte> utf8Bytes = stackalloc byte[maxLen];
            ReadOnlySpan<char> chars = stackalloc char[1] { value };

            var len = UTF8NoBom.GetBytes(chars, utf8Bytes);

            TryGrow(len * repeatCount);

            for(var i = 0; i < repeatCount; i++)
            {
                utf8Bytes.CopyTo(GetSpan(len));
                Advance(len);
            }
        }
    }

    /// <summary>Appends a contiguous region of arbitrary memory to this instance.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(scoped ReadOnlySpan<char> value)
    {
        var maxLen = UTF8NoBom.GetMaxByteCount(value.Length);

        if(buffer!.Length - index < maxLen)
        {
            Grow(maxLen);
        }

        index += UTF8NoBom.GetBytes(value, buffer.AsSpan(index));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(scoped ReadOnlySpan<byte> value)
    {
        if((buffer!.Length - index) < value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(buffer.AsSpan(index));

        index += value.Length;
    }

    #endregion

    #region Append Line

    /// <summary>Appends the default line terminator to the end of this instance.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine()
    {
        if(SharedConstants.NewLine.CRLF)
        {
            if(buffer!.Length - index < 2)
            {
                Grow(2);
            }

            buffer[index] = SharedConstants.Json.CarriageReturn;// \r
            buffer[index + 1] = SharedConstants.Json.LineFeed; // \n
            index += 2;
        }
#pragma warning disable CS0162 
        else
        {
            if(buffer!.Length - index < 1)
            {
                Grow(1);
            }

            buffer[index] = SharedConstants.Json.LineFeed;
            index += 1;
        }
#pragma warning restore CS0162 
    }

    /// <summary>Appends the string representation of a specified value followed by the default line terminator to the end of this instance.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(char value)
    {
        Append(value);
        AppendLine();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(char value, int repeatCount)
    {
        Append(value, repeatCount);
        AppendLine();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(scoped ReadOnlySpan<char> value)
    {
        Append(value);
        AppendLine();
    }

    #endregion

    #region Copy To

    /// <summary>Copy inner buffer to the bufferWriter.</summary>
    public void CopyTo(IBufferWriter<byte> bufferWriter)
    {
        var destination = bufferWriter.GetSpan(index);
        _ = TryCopyTo(destination, out var written);
        bufferWriter.Advance(written);
    }

    /// <summary>Copy inner buffer to the destination span.</summary>
    public bool TryCopyTo(Span<byte> destination, out int bytesWritten)
    {
        if(destination.Length < index)
        {
            bytesWritten = 0;
            return false;
        }

        bytesWritten = index;
        buffer.AsSpan(0, index).CopyTo(destination);
        return true;
    }

    #endregion

    /// <summary>Write inner buffer to stream.</summary>
    public Task WriteToAsync(Stream stream)
    {
        return stream.WriteAsync(buffer, 0, index);
    }

    /// <summary>Write inner buffer to stream.</summary>
    public Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
    {
        return stream.WriteAsync(buffer, 0, index, cancellationToken);
    }

    /// <summary>Encode the innner utf8 buffer to a System.String.</summary>
    public override string ToString()
    {
        if(index == 0)
        {
            return string.Empty;
        }

        return UTF8NoBom.GetString(buffer, 0, index);
    }

    #region IBufferWriter

    /// <summary>IBufferWriter.GetMemory.</summary>
    public Memory<byte> GetMemory(int sizeHint)
    {
        if((buffer!.Length - index) < sizeHint)
        {
            Grow(sizeHint);
        }

        return buffer.AsMemory(index);
    }

    /// <summary>IBufferWriter.GetSpan.</summary>
    public Span<byte> GetSpan(int sizeHint)
    {
        if((buffer!.Length - index) < sizeHint)
        {
            Grow(sizeHint);
        }

        return buffer.AsSpan(index);
    }

    /// <summary>IBufferWriter.Advance.</summary>
    public void Advance(int count)
    {
        index += count;
    }

    #endregion

    [DoesNotReturn]
    private static void ThrowNestedException()
    {
        throw new InvalidOperationException($"A nested call with `notNested: true`, or Either You forgot to call {nameof(Utf8ValueStringBuilder)}.Dispose() of  in the past.");
    }
}
