namespace OneI.Logable;

using System;
using System.IO;

public class StreamCounter : Stream
{
    private readonly Stream _stream;

    public StreamCounter(Stream stream)
    {
        _stream = stream;

        Count = stream.Length;
    }

    public long Count { get; private set; }

    public override void Flush() => _stream.Flush();
    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override long Length => _stream.Length;
    public override long Position
    {
        get => _stream.Position;
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new InvalidOperationException($"Seek operations are not available through `{nameof(StreamCounter)}`");
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);

        if(value < Count)
        {
            Count = _stream.Length;
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);

        Count += count;
    }
}
