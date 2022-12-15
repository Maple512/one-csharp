namespace OneI.Logable;

using System;
using OneI.Logable.Configurations;
using OneI.Logable.Rendering;

public static class LoggerSinkConfigurationExtensions
{
    public static ILoggerConfiguration File(
        this ILoggerSinkConfiguration configuration,
        LogFileOptions options)
    {
        return configuration.Use(CreateSink(options));
    }

    private static ILoggerSink CreateSink(LogFileOptions options)
    {
        ILoggerSink sink;
        if(options.Frequency != RollFrequency.Naver)
        {
            sink = new RollFileSink(options.Path,
                options.Frequency,
                options.TextRenderers,
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
                options.TextRenderers,
                options.FileSizeMaxBytes,
                options.Encoding);
        }
        else
        {
            sink = new FileSink(
                options.Path,
                options.TextRenderers,
                options.FileSizeMaxBytes,
                options.Encoding,
                options.Buffered);
        }

        return sink;
    }
}

public class LogFileOptions
{
    private const string DefaultTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

    /// <param name="path">日志路径</param>
    /// <param name="template">日志消息模版，默认格式：<code>{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}</code></param>
    public LogFileOptions(string path, string? template = null, IFormatProvider? formatProvider = null)
        : this(path, new TextRendererProvider(template ?? DefaultTemplate, formatProvider))
    {
    }

    public LogFileOptions(string path, ITextRendererProvider rendererProvider)
    {
        Path = path;

        TextRenderers = new()
        {
            Check.NotNull(rendererProvider)
        };
    }

    /// <summary>
    /// 日志路径，如：./Logs/*.txt
    /// </summary>
    public string Path { get; }

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

    public List<ITextRendererProvider> TextRenderers { get; }

    /// <summary>
    /// 表示按条件对指定文本进行渲染
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="textRenderer"></param>
    public void RenderWhen(Func<LoggerContext, bool> condition, ITextRenderer textRenderer)
    {
        TextRenderers.Insert(0, new ConditionalRendererProvider(condition, textRenderer));
    }

    /// <summary>
    /// 表示按条件对指定文本进行渲染
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="template"></param>
    /// <param name="formatProvider"></param>
    public void RenderWhen(Func<LoggerContext, bool> condition, string template, IFormatProvider? formatProvider = null)
    {
        TextRenderers.Insert(0, new ConditionalRendererProvider(condition, new TextTemplateRenderer(template, formatProvider)));
    }
}
