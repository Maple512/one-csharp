namespace OneI.Logable;

public readonly struct LogLevelMap
{
    private readonly Dictionary<string, LogLevelRange> _overrides;

    private readonly LogLevelRange _range;

    public const LogLevel MaximumLevelDefault = LogLevel.Fatal;

    public LogLevelMap(LogLevel minimum, LogLevel? maximum = null)
    {
        _range = new LogLevelRange(minimum, maximum);
        _overrides = new Dictionary<string, LogLevelRange>();
    }

    public LogLevelMap()
    {
        _overrides = new();
        _range = new();
    }

    public LogLevel MinimumLevel => _range.Minimum;

    public LogLevel? MaximumLevel => _range.Maximum;

    public LogLevelMap Minimum(LogLevel level)
    {
        _range.Min(level);

        return this;
    }

    public LogLevelMap Maximum(LogLevel? level)
    {
        _range.Max(level);

        return this;

    }

    internal LogLevelMap Override(LogLevelMap @new)
    {
        _range.Min(@new.MinimumLevel);
        _range.Max(@new.MaximumLevel);

        foreach(var item in @new._overrides)
        {
            Override(item.Key, item.Value.Minimum, item.Value.Maximum);
        }

        return this;
    }

    public LogLevelMap Override(string sourceContext, LogLevel minimum, LogLevel? maximum)
    {
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

    /// <summary>
    /// 是否启用日志等级
    /// </summary>
    /// <param name="context"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel level, string? context = null)
    {
        if(context.NotNullOrWhiteSpace())
        {
            foreach(var item in _overrides)
            {
                if(context!.StartsWith(item.Key)
                    && (context.Length == item.Key.Length || context[item.Key.Length] == '.'))
                {
                    return item.Value.IsEnabled(level);
                }
            }
        }

        return _range.IsEnabled(level);
    }
}
