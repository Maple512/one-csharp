namespace OneI.Logable;

public class LogFileOptions
{
    private ILoggerRenderer? _renderer;
    private long sizeLimit = 0;
    private int countLimit = 1;
    private int bufferSize = 0;

    public LogFileOptions(string path)
    {
        Check.NotNullOrWhiteSpace(path);

        Path = path;
    }

    public string Path { get; }

    /// <summary>
    /// 文件最大长度（单位：字节）
    /// </summary>
    public long SizeLimit
    {
        get => sizeLimit;
        set
        {
            if(value < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(value));
            }

            sizeLimit = value;
        }
    }
    /// <summary>
    /// 文件编码
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// 文件滚动周期，默认：<see cref="RollFrequency.Hour"/>
    /// </summary>
    public RollFrequency Frequency { get; set; } = RollFrequency.Hour;

    /// <summary>
    /// 数量限制
    /// </summary>
    public int CountLimit
    {
        get => countLimit;
        set
        {
            if(value < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(value));
            }

            countLimit = value;
        }
    }
    /// <summary>
    /// 文件过期时间
    /// </summary>
    public TimeSpan ExpiredTime { get; set; }

    /// <summary>
    /// 用于缓冲的缓冲区 FileStream 的大小。 默认缓冲区大小为 4096。 0 或 1 表示应禁用缓冲。 不允许负值
    /// </summary>
    public int BufferSize
    {
        get => bufferSize;
        set
        {
            if(value < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(value));
            }

            bufferSize = value;
        }
    }

    /// <summary>
    /// 日志文本渲染器
    /// </summary>
    public ILoggerRenderer Renderer
    {
        get => _renderer ?? new LoggerRenderer(null);
        set => _renderer = value;
    }
}
