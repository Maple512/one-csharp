namespace OneI.Logable;

/// <summary>
/// 表示描述日志等级范围
/// </summary>
internal readonly struct LogLevelRange
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogLevelRange"/> class.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="maximum">The maximum.</param>
    public LogLevelRange(LogLevel minimum, LogLevel? maximum = null)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// Gets the minimum.
    /// </summary>
    public LogLevel Minimum { get; }

    /// <summary>
    /// Gets the maximum.
    /// </summary>
    public LogLevel? Maximum { get; }
}
