namespace OneI.Logable;

internal class LogLevelMap
{
    public const LogLevel MinimumLevelDefault = LogLevel.Verbose;

    public const LogLevel MaximumLevelDefault = LogLevel.Fatal;

    private readonly Dictionary<string, LogLevelRange> _overrides;

    private LogLevelRange _range;

    public LogLevelMap()
    {
        _overrides = new(StringComparer.InvariantCulture);
        _range = new(MinimumLevelDefault, MaximumLevelDefault);
    }

    public LogLevel MinimumLevel => _range.Minimum;

    public LogLevel? MaximumLevel => _range.Maximum;

    public void Override(LogLevel minimum, LogLevel maximum)
    {
        _range = new(minimum, maximum);
    }

    public LogLevelMap Override(string sourceContext, LogLevel minimum, LogLevel maximum)
    {
        Check.NotNullOrWhiteSpace(sourceContext);

        _overrides[sourceContext] = new LogLevelRange(minimum, maximum);

        return this;
    }

    public bool IsEnabled(LogLevel level)
    {
        return _range.IsEnabled(level);
    }

    public LogLevelRange GetEffectiveLevel(string? context = null)
    {
        if(context is { Length: > 0 })
        {
            foreach(var item in _overrides)
            {
                if(context.AsSpan().StartsWith(item.Key)
                    && (context.Length == item.Key.Length || context[item.Key.Length] == '.'))
                {
                    return item.Value;
                }
            }
        }

        return _range;
    }
}
