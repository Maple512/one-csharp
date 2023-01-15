namespace OneI.Logable;

internal readonly struct LogLevelRange
{
    public LogLevelRange(LogLevel minimum, LogLevel maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public LogLevel Minimum { get; }

    public LogLevel Maximum { get; }

    public bool IsEnabled(LogLevel level)
    {
        var sl = (sbyte)level;
        if((sbyte)Minimum > (sbyte)level)
        {
            return false;
        }

        if((sbyte)Maximum < sl)
        {
            return false;
        }

        return true;
    }
}
