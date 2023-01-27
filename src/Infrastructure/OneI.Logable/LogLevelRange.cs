namespace OneI.Logable;

internal readonly struct LogLevelRange
{
    public LogLevelRange(LogLevel minimum, LogLevel maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public LogLevel Minimum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public LogLevel Maximum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
}
