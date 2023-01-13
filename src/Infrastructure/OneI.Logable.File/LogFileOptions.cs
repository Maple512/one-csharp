namespace OneI.Logable;

using OneI.Logable.Rendering;

public class LoggerFileOptionsBase
{
    public LoggerFileOptionsBase(string path)
    {
        Path = Check.NotNullOrWhiteSpace(path);
    }

    /// <summary>
    /// 文件路径。如：./Logs/log.log
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets or sets the format provider.
    /// </summary>
    public IFormatProvider? FormatProvider { get; init; }

    /// <summary>
    /// 文件最大长度
    /// </summary>
    public long? FileSizeMaxBytes { get; init; }

    /// <summary>
    /// 文件编码
    /// </summary>
    public Encoding? Encoding { get; init; }

    /// <summary>
    /// 是否定期刷新缓冲区
    /// </summary>
    public TimeSpan? FlushRegularly { get; init; }

    public ILoggerRenderer GetLoggerRenderer() => new LoggerRenderer(FormatProvider);
}

public class LoggerFileOptions : LoggerFileOptionsBase
{
    public LoggerFileOptions(string path) : base(path)
    {
    }

    public bool Buffered { get; set; }
}

public class LoggerSharedFileOptions : LoggerFileOptionsBase
{
    public LoggerSharedFileOptions(string path) : base(path)
    {
    }
}

public class LoggerRollFileOptions : LoggerFileOptions
{
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
