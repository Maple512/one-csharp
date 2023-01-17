namespace OneI.Logable;

using OneI.Logable.Rendering;
using OneI.Logable.Templatizations;
using OneI.Logable.Templatizations.Tokenizations;

public class LogFileOptions
{
    private ILoggerRenderer? _renderer;

    public LogFileOptions(string path)
    {
        Check.NotNullOrWhiteSpace(path);

        Tokens = TemplateParser.Parse(path);
    }

    public IReadOnlyList<ITemplateToken> Tokens { get; }

    public IFormatProvider? FormatProvider { get; init; }

    /// <summary>
    /// 文件最大长度
    /// </summary>
    public long? SizeLimit { get; init; }

    /// <summary>
    /// 文件编码
    /// </summary>
    public Encoding Encoding { get; init; } = Encoding.UTF8;

    /// <summary>
    /// 文件滚动周期，默认：<see cref="RollFrequency.Hour"/>
    /// </summary>
    public RollFrequency Frequency { get; set; } = RollFrequency.Hour;

    /// <summary>
    /// 数量限制
    /// </summary>
    public int? CountLimit { get; set; }

    /// <summary>
    /// 时间限制
    /// </summary>
    public TimeSpan? ExpiredTime { get; set; }

    /// <summary>
    /// 日志文本渲染器
    /// </summary>
    public ILoggerRenderer Renderer
    {
        get => _renderer ?? new LoggerRenderer(FormatProvider);
        init => _renderer = value;
    }
}
