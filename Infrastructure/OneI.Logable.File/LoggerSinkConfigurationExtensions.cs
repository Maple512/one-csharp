namespace OneI.Logable;

using System;
using OneI.Logable.Configurations;
using OneI.Logable.Templating.Rendering;

public static class LoggerSinkConfigurationExtensions
{
    public static ILoggerConfiguration File(
        this ILoggerSinkConfiguration configuration,
        LogFileOptions options)
    {
        ILoggerSink sink;
        if(options.Frequency != RollFrequency.Naver)
        {
            sink = new RollFileSink(options.Path,
                options.Frequency,
                options.TextRenderer,
                options.FileSizeMaxBytes,
                options.RetainedFileCountMax,
                options.RetainedFileTimeMax,
                options.Encoding,
                options.Buffered,
                options.IsShared);
        }
        else if(options.IsShared)
        {
            sink = new SharedFileSink(
                options.Path,
                options.TextRenderer,
                options.FileSizeMaxBytes,
                options.Encoding);
        }
        else
        {
            sink = new FileSink(
                options.Path,
                options.TextRenderer,
                options.FileSizeMaxBytes,
                options.Encoding,
                options.Buffered);
        }

        return configuration.Use(sink);
    }
}

public class LogFileOptions
{
    private const string DefaultTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

    /// <param name="path">日志路径</param>
    /// <param name="template">日志消息模版，默认格式：<code>{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}</code></param>
    public LogFileOptions(string path, string? template = null, IFormatProvider? formatProvider = null)
        : this(path, new TextTemplateRenderer(template ?? DefaultTemplate, formatProvider))
    {
    }

    public LogFileOptions(string path, ITextRenderer textRenderer)
    {
        Path = path;

        TextRenderer = textRenderer;
    }

    /// <summary>
    /// 日志路径，如：./Logs/*.txt
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 文本渲染器。默认：<see cref="TextTemplateRenderer"/>
    /// </summary>
    public ITextRenderer TextRenderer { get; init; }

    /// <summary>
    /// 文件最大长度
    /// </summary>
    public long? FileSizeMaxBytes { get; init; }

    /// <summary>
    /// 是否开启缓冲区
    /// </summary>
    public bool Buffered { get; init; }

    public bool IsSilent { get; init; }

    /// <summary>
    /// 是否为共享文件
    /// </summary>
    public bool IsShared { get; init; }

    /// <summary>
    /// 文件编码
    /// </summary>
    public Encoding? Encoding { get; init; }

    /// <summary>
    /// 文件滚动周期
    /// </summary>
    public RollFrequency Frequency { get; init; }

    /// <summary>
    /// 最大日志文件的保留数量
    /// </summary>
    public int? RetainedFileCountMax { get; init; }

    /// <summary>
    /// 最大日志文件的保留时间
    /// </summary>
    public TimeSpan? RetainedFileTimeMax { get; init; }
}
