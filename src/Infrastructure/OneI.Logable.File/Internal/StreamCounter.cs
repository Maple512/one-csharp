namespace OneI.Logable;

/// <summary>
/// 表示一个<see cref="Stream"/>的长度计数器
/// </summary>
internal class StreamCounter : Stream
{
    private readonly Stream _stream;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamCounter"/> class.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public StreamCounter(Stream stream)
    {
        _stream = stream;
        TotalLength = stream.Length;
    }

    /// <summary>
    /// 总长度
    /// </summary>
    public long TotalLength { get; private set; }

    /// <summary>
    /// Gets a value indicating whether can read.
    /// </summary>
    public override bool CanRead => _stream.CanRead;
    /// <summary>
    /// Gets a value indicating whether can seek.
    /// </summary>
    public override bool CanSeek => _stream.CanSeek;
    /// <summary>
    /// Gets a value indicating whether can write.
    /// </summary>
    public override bool CanWrite => _stream.CanWrite;
    /// <summary>
    /// Gets the length.
    /// </summary>
    public override long Length => _stream.Length;
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public override long Position { get => _stream.Position; set => throw new NotSupportedException(); }

    /// <summary>
    /// Flushes the.
    /// </summary>
    public override void Flush()
    {
        _stream.Flush();
    }

    /// <summary>
    /// Reads the.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="count">The count.</param>
    /// <returns>An int.</returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Seeks the.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="origin">The origin.</param>
    /// <returns>A long.</returns>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Sets the length.
    /// </summary>
    /// <param name="value">The value.</param>
    public override void SetLength(long value)
    {
        _stream.SetLength(value);
        if(value < TotalLength)
        {
            TotalLength = _stream.Length;
        }
    }

    /// <summary>
    /// Writes the.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="count">The count.</param>
    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);

        TotalLength += count;
    }
}
