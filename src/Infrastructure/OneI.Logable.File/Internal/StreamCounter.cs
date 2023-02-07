namespace OneI.Logable.Internal;

internal class StreamCounter : Stream
{
    private readonly Stream _stream;

    public StreamCounter(Stream stream)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        CountedLength = stream.Length;
    }

    public long CountedLength { get; private set; }

    public override bool CanRead
    {
        get
        {
            return false;
        }
    }

    public override bool CanSeek
    {
        get
        {
            return _stream.CanSeek;
        }
    }

    public override bool CanWrite
    {
        get
        {
            return true;
        }
    }

    public override long Length
    {
        get
        {
            return _stream.Length;
        }
    }

    public override long Position
    {
        get
        {
            return _stream.Position;
        }

        set
        {
            throw new NotSupportedException();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            _stream.Dispose();
        }

        base.Dispose(disposing);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
        CountedLength += count;
    }

    public override void Flush() => _stream.Flush();

    public override long Seek(long offset, SeekOrigin origin)
        => throw new InvalidOperationException($"Seek operations are not available through `{nameof(StreamCounter)}`.");

    public override void SetLength(long value)
    {
        _stream.SetLength(value);

        if(value < CountedLength)
        {
            // RollFile is now shorter and our position has changed to _stream.Length
            CountedLength = _stream.Length;
        }
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
