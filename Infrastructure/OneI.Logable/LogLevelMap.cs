namespace OneI.Logable;

/// <summary>
/// The log level map.
/// </summary>
public readonly struct LogLevelMap
{
    /// <summary>
    /// The overrides.
    /// </summary>
    private readonly Dictionary<string, LogLevelRange> _overrides;

    /// <summary>
    /// The range.
    /// </summary>
    private readonly LogLevelRange _range;

    public const LogLevel MaximumLevel = LogLevel.Fatal;

    public LogLevelMap()
    {
        _overrides = new();
        _range = new();
    }

    public void Minimum(LogLevel level)
    {
        _range.Min(level);
    }

    public void Maximum(LogLevel level)
    {
        _range.Max(level);
    }

    public void Override(string sourceContext, LogLevel minimum, LogLevel? maximum)
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
