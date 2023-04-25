namespace OneI.Openable.Connections.Quic;

public sealed class QuicTransportOptions
{
    private long _streamErrorCodeDefault;
    private long _closeErrorCodeDefault;

    /// <summary>
    /// 每个连接并发双向流的最大数量
    /// </summary>
    public int BidirectionalStreamMaxCount { get; set; } = 100;

    /// <summary>
    /// 每个连接并发入站单向流的最大数量
    /// </summary>
    public int UnidirectionalStreamMaxCount { get; set; } = 10;

    public long? ReadBufferMaxLength { get; set; } = 1024 * 1024;

    public long? WriteBufferMaxLength { get; set; } = 64 * 1024;

    public int PendingConnectionQueueMaxLength { get; set; } = 512;

    public long StreamErrorCodeDefault
    {
        get => _streamErrorCodeDefault;
        set
        {
            ValidateErrorCode(value);

            _streamErrorCodeDefault = value;
        }
    }

    public long CloseErrorCodeDefault
    {
        get => _closeErrorCodeDefault;
        set
        {
            ValidateErrorCode(value);

            _closeErrorCodeDefault = value;
        }
    }

    private static void ValidateErrorCode(long code)
    {
        const long MinErrorCode = 0, MaxErrorCode = (1L << 62) - 1;

        if(code is < MinErrorCode
            or > MaxErrorCode)
        {
            throw new OneIArgumentOutOfRangeException<long>(code, nameof(code), $"A value between {MinErrorCode} and {MaxErrorCode} is required.");
        }
    }
}
