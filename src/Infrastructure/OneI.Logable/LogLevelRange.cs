namespace OneI.Logable;

/// <summary>
/// 表示描述日志等级范围
/// </summary>
internal readonly struct LogLevelRange
{
    public LogLevelRange(LogLevel minimum, LogLevel? maximum = null)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public LogLevel Minimum { get; }

    public LogLevel? Maximum { get; }
}
