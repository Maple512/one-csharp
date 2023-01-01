namespace OneI.Logable;

using OneI.Logable.Rendering;
using OneI.Textable;
/// <summary>
/// The logger file options base.
/// </summary>

public class LoggerFileOptionsBase
{
    private readonly List<Func<LoggerContext, ILoggerRenderer?>> _rendererProviders;
    /// <summary>
    /// The default template.
    /// </summary>
    private const string DefaultTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}";

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerFileOptionsBase"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    public LoggerFileOptionsBase(string path)
    {
        Path = Check.NotNullOrWhiteSpace(path);
        _rendererProviders = new();
    }

    /// <summary>
    /// 文件路径。如：./Logs/log.log
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 输出模版，默认：<code>{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}</code>
    /// </summary>
    public string? Template { get; set; } = DefaultTemplate;

    /// <summary>
    /// Gets or sets the format provider.
    /// </summary>
    public IFormatProvider? FormatProvider { get; set; }

    /// <summary>
    /// 文件最大长度
    /// </summary>
    public long? FileSizeMaxBytes { get; set; }

    /// <summary>
    /// 文件编码
    /// </summary>
    public Encoding? Encoding { get; set; }

    /// <summary>
    /// 是否定期刷新缓冲区
    /// </summary>
    public TimeSpan? FlushRegularly { get; set; }

    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <returns>An ITextRendererProvider.</returns>
    internal ITextRendererProvider GetProvider()
    {
        if(Template.NotNullOrWhiteSpace())
        {
            _rendererProviders.Add(_ => new LoggerRenderer(TextTemplate.Create(Template), FormatProvider));
        }

        return new TextRendererProvider(_rendererProviders);
    }

    /// <summary>
    /// 表示按条件对指定文本进行渲染
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="textRenderer"></param>
    public void RenderWhen(Func<LoggerContext, bool> condition, ILoggerRenderer textRenderer)
    {
        _rendererProviders.Insert(0, context => condition(context) ? textRenderer : null);
    }

    /// <summary>
    /// 表示按条件对指定文本进行渲染
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="template"></param>
    /// <param name="formatProvider"></param>
    public void RenderWhen(Func<LoggerContext, bool> condition, string template, IFormatProvider? formatProvider = null)
    {
        RenderWhen(condition, new LoggerRenderer(TextTemplate.Create(template), formatProvider));
    }
}
/// <summary>
/// The logger file options.
/// </summary>

public class LoggerFileOptions : LoggerFileOptionsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerFileOptions"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    public LoggerFileOptions(string path) : base(path)
    {
    }

    /// <summary>
    /// 是否开启缓冲区
    /// </summary>
    public bool Buffered { get; set; }
}
/// <summary>
/// The logger shared file options.
/// </summary>

public class LoggerSharedFileOptions : LoggerFileOptionsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerSharedFileOptions"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    public LoggerSharedFileOptions(string path) : base(path)
    {
    }
}
/// <summary>
/// The logger roll file options.
/// </summary>

public class LoggerRollFileOptions : LoggerFileOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerRollFileOptions"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    public LoggerRollFileOptions(string path) : base(path)
    {
    }

    /// <summary>
    /// 是否为共享文件
    /// </summary>
    public bool IsShared { get; set; }

    /// <summary>
    /// 文件滚动周期，默认：<see cref="RollFrequency.Hour"/>
    /// </summary>
    public RollFrequency Frequency { get; set; } = RollFrequency.Hour;

    /// <summary>
    /// 最大日志文件的保留数量
    /// </summary>
    public int? RetainedFileCountMax { get; set; }

    /// <summary>
    /// 最大日志文件的保留时间
    /// </summary>
    public TimeSpan? RetainedFileTimeMax { get; set; }
}
