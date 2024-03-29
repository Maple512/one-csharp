namespace OneI.Logable.Internal;

using System.Buffers;
using Microsoft.Win32.SafeHandles;

internal class FileWriter : TextWriter
{
    private readonly Encoding _encoding;
    private SafeFileHandle _file;

    public FileWriter(SafeFileHandle file, Encoding encoding, long position = 0)
    {
        _file = file;
        _encoding = encoding;
        Position = position;
    }

    public override Encoding Encoding => _encoding;

    public long Position { get; private set; }

    public override Task FlushAsync() => Task.CompletedTask;

    protected override void Dispose(bool disposing) => _file.Dispose();

    internal void ResetNewFile(SafeFileHandle file)
    {
        _file.Dispose();
        _file = default!;
        _file = file;
        Position = 0;
    }

    #region Write

    public override void Write(char value)
    {
        RandomAccess.Write(_file, new ReadOnlySpan<byte>((byte)value), Position);
        Position++;
    }

    public override void Write(char[] buffer, int index, int count)
    {
        var bytes = _encoding.GetBytes(buffer, index, count);

        RandomAccess.Write(_file, bytes, Position);

        Position += bytes.Length;
    }

    public override void Write(ReadOnlySpan<char> buffer)
    {
        var byteCount = _encoding.GetByteCount(buffer);

        scoped Span<byte> span = stackalloc byte[byteCount];

        var written = _encoding.GetBytes(buffer, span);

        RandomAccess.Write(_file, span, Position);

        Position += written;
    }

    public override void Write(string? value)
    {
        if(value is { Length: > 0, })
        {
            Write(value.AsSpan());
        }
    }

    #endregion

    #region Write Async

    public override async Task WriteAsync(char value)
    {
        var chars = ArrayPool<char>.Shared.Rent(1);
        try
        {
            chars.SetValue(value, 0);

            await WriteAsync(chars, 0, 1);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(chars);
        }
    }

    public override async Task WriteAsync(char[] buffer, int index, int count)
    {
        var bytes = _encoding.GetBytes(buffer, index, count);

        await RandomAccess.WriteAsync(_file, bytes, Position);

        Position += bytes.Length;
    }

    public override async Task WriteAsync(string? value)
    {
        if(value is { Length: > 0, })
        {
            await WriteAsync(value.AsMemory());
        }
    }

    public override async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
        {
            await Task.FromCanceled(cancellationToken);

            return;
        }

        var byteCount = _encoding.GetByteCount(buffer.Span);

        var chars = ArrayPool<byte>.Shared.Rent(byteCount);
        try
        {
            var written = _encoding.GetBytes(buffer.Span, chars);

            await RandomAccess.WriteAsync(_file, chars, Position, cancellationToken);

            Position += written;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(chars);
        }
    }

    #endregion

    #region Write Line Async

    public override async Task WriteLineAsync(char value)
    {
        await WriteAsync(value);

        await WriteLineAsync();
    }

    public override async Task WriteLineAsync(string? value)
    {
        await WriteAsync(value);

        await WriteLineAsync();
    }

    public override async Task WriteLineAsync(char[] buffer, int index, int count)
    {
        await WriteAsync(buffer, index, count);

        await WriteLineAsync();
    }

    public override async Task WriteLineAsync(ReadOnlyMemory<char> buffer
                                              , CancellationToken cancellationToken = default)
    {
        await WriteAsync(buffer, cancellationToken);

        await WriteLineAsync();
    }

    #endregion
}
