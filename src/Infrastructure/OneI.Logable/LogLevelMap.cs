namespace OneI.Logable;

internal class LogLevelMap
{
    private const LogLevel MinimumLevelDefault = LogLevel.Verbose;

    public const LogLevel MaximumLevelDefault = LogLevel.Fatal;

    private readonly Dictionary<string, LogLevelRange> _overrides;

    private LogLevelRange _range;

    public LogLevelMap()
    {
        _overrides = new(StringComparer.InvariantCulture);
        _range = new(MinimumLevelDefault, MaximumLevelDefault);
    }

    public LogLevel Minimum => _range.Minimum;
    public LogLevel Maximum => _range.Maximum;

    public void Override(LogLevel minimum, LogLevel maximum)
    {
        _range = new(minimum, maximum);
    }

    public void Override(string sourceContext, LogLevel minimum, LogLevel maximum)
    {
        Check.NotNullOrWhiteSpace(sourceContext);

        _overrides[sourceContext] = new LogLevelRange(minimum, maximum);
    }

    public bool IsEnabled(LogLevel level)
    {
        var sl = (sbyte)level;
        if((sbyte)Minimum > (sbyte)level)
        {
            return false;
        }

        return (sbyte)Maximum >= sl;
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
