namespace OneI.Logable;

internal readonly struct LogLevelRange
{
    public LogLevelRange(LogLevel? minimum, LogLevel? maximum = null)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public LogLevel? Minimum { get; }

    public LogLevel? Maximum { get; }
}
