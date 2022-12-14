namespace OneI.Logable;

/// <summary>
/// 表示日志等级的范围
/// </summary>
public struct LogLevelRange
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogLevelRange"/> class.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="maximum">The maximum.</param>
    public LogLevelRange(LogLevel? minimum = LogLevel.Information, LogLevel? maximum = null)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// Gets the minimum.
    /// </summary>
    /// <value>A LogLevel? .</value>
    public LogLevel? Minimum { get; private set; }

    /// <summary>
    /// Gets the maximum.
    /// </summary>
    /// <value>A LogLevel? .</value>
    public LogLevel? Maximum { get; private set; }

    public void Min(LogLevel level)
    {
        if(Maximum.HasValue
            && Maximum.Value < level)
        {
            throw new ArgumentOutOfRangeException(nameof(Minimum), "The minimum log level cannot be greater than the maximum.");
        }

        Minimum = level;
    }

    public void Max(LogLevel level)
    {
        if(Minimum.HasValue
            && Minimum.Value > level)
        {
            throw new ArgumentOutOfRangeException(nameof(Maximum), "The maximum value of the log level cannot be smaller than the minimum value.");
        }

        Maximum = level;
    }

    /// <summary>
    /// Is enabled.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <returns>A bool.</returns>
    public bool IsEnabled(LogLevel level)
    {
        if(Minimum.HasValue
            && Minimum.Value < level)
        {
            return false;
        }

        if(Maximum.HasValue
            && Maximum.Value > level)
        {
            return false;
        }

        return true;
    }
}
