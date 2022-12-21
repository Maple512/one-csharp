namespace OneI.Logable;

/// <summary>
/// 表示一个<see cref="Stream"/>的长度计数器
/// </summary>
internal class StreamCounter : Stream
{
    private readonly Stream _stream;

    public StreamCounter(Stream stream)
    {
        _stream = stream;
        TotalLength = stream.Length;
    }

    /// <summary>
    /// 总长度
    /// </summary>
    public long TotalLength { get; private set; }

    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override long Length => _stream.Length;
    public override long Position { get => _stream.Position; set => throw new NotSupportedException(); }

    public override void Flush()
    {
        _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);
        if(value < TotalLength)
        {
            TotalLength = _stream.Length;
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);

        TotalLength += count;
    }
}
