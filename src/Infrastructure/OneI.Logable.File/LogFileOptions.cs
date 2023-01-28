namespace OneI.Logable;

using Rendering;
using Templatizations;
using Templatizations.Tokenizations;

public class LogFileOptions
{
    private ILoggerRenderer? _renderer;

    public LogFileOptions(string path)
    {
        Check.NotNullOrWhiteSpace(path);

        Tokens = TemplateParser.Parse(path.AsMemory());
    }

    public IEnumerable<ITemplateToken> Tokens { get; }

    public IFormatProvider? FormatProvider { get; set; }

    /// <summary>
    /// 文件最大长度（单位：字节）
    /// </summary>
    public ulong SizeLimit { get; set; } = 0;

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
    public uint CountLimit { get; set; } = 1;

    /// <summary>
    /// 文件过期时间
    /// </summary>
    public TimeSpan ExpiredTime { get; set; }

    /// <summary>
    /// 用于缓冲的缓冲区 FileStream 的大小。 默认缓冲区大小为 4096。 0 或 1 表示应禁用缓冲。 不允许负值
    /// </summary>
    public uint BufferSize { get; set; } = 4096;

    /// <summary>
    /// 日志文本渲染器
    /// </summary>
    public ILoggerRenderer Renderer
    {
        get => _renderer ?? new LoggerRenderer(FormatProvider);
        set => _renderer = value;
    }
}
