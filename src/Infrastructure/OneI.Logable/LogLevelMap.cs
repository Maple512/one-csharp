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
        _range = new();
    }

    public LogLevel MinimumLevel => _range.Minimum ?? MinimumLevelDefault;

    public LogLevel? MaximumLevel => _range.Maximum;

    public LogLevelMap Minimum(LogLevel level)
    {
        if(MaximumLevel.HasValue
            && MaximumLevel.Value < level)
        {
            throw new ArgumentOutOfRangeException(nameof(Minimum), "The minimum log level cannot be greater than the maximum.");
        }

        _range = new LogLevelRange(level, _range.Maximum);

        return this;
    }

    public LogLevelMap Maximum(LogLevel? level)
    {
        if(MinimumLevel > level)
        {
            throw new ArgumentOutOfRangeException(nameof(Maximum), "The maximum value of the log level cannot be smaller than the minimum value.");
        }

        _range = new(_range.Minimum, level);

        return this;
    }

    public LogLevelMap Override(string sourceContext, LogLevel minimum, LogLevel? maximum)
    {
        Check.NotNullOrWhiteSpace(sourceContext);

        maximum ??= _range.Maximum ?? MaximumLevel;

        var range = new LogLevelRange(minimum, maximum);
        if(_overrides.ContainsKey(sourceContext))
        {
            _overrides[sourceContext] = range;
        }
        else
        {
            _overrides.Add(sourceContext, range);
        }

        return this;
    }

    public bool IsEnabled(LogLevel level, string? context = null)
    {
        if(context.NotNullOrWhiteSpace())
        {
            foreach(var item in _overrides)
            {
                if(context!.StartsWith(item.Key)
                    && (context.Length == item.Key.Length || context[item.Key.Length] == '.'))
                {
                    return IsEnabled(level);
                }
            }
        }

        return IsEnabled(_range, level);
    }

    private static bool IsEnabled(LogLevelRange range, LogLevel level)
    {
        if(range.Minimum > level)
        {
            return false;
        }

        if(range.Maximum.HasValue
            && range.Maximum.Value < level)
        {
            return false;
        }

        return true;
    }
}
