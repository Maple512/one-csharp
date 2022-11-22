namespace OneI.Logable.Internal;

using System;
using System.Collections.Generic;
using System.Linq;

internal class LevelOverrideMapper
{
    private readonly LogLevel _defaultMinimunLevel;
    private readonly LogLevelSwitch? _defaultLevelSwitch;
    private readonly IEnumerable<OverrideContext> _overrides;

    public LevelOverrideMapper(
        IDictionary<string, LogLevelSwitch> overrides,
        LogLevel minimunLevel,
        LogLevelSwitch? levelSwitch)
    {
        _defaultMinimunLevel = minimunLevel;
        _defaultLevelSwitch = levelSwitch;

        _overrides = overrides.OrderByDescending(x => x.Key)
            .Select(x => new OverrideContext(x.Key, x.Value));
    }

    /// <summary>
    /// 获取实际有效的日志等级
    /// </summary>
    /// <param name="sourceName"></param>
    /// <param name="minimumLevel"></param>
    /// <param name="levelSwitch"></param>
    public void GetEfffectiveLevel(
        ReadOnlySpan<char> sourceName,
        out LogLevel minimumLevel,
        out LogLevelSwitch? levelSwitch)
    {
        foreach(var item in _overrides)
        {
            if(sourceName.StartsWith(item.SourceName)
                && (sourceName.Length == item.SourceName.Length || sourceName[item.SourceName.Length] == '.'))
            {
                minimumLevel = LogLevelSwitch.Minimum;
                levelSwitch = item.LevelSwitch;
                return;
            }
        }

        minimumLevel = _defaultMinimunLevel;
        levelSwitch = _defaultLevelSwitch;
    }

    private readonly struct OverrideContext
    {
        public OverrideContext(string sourceName, LogLevelSwitch levelSwitch)
        {
            SourceName = sourceName;
            LevelSwitch = levelSwitch;
        }

        public string SourceName { get; }

        public LogLevelSwitch LevelSwitch { get; }
    }
}
