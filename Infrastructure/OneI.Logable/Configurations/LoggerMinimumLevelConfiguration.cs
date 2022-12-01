namespace OneI.Logable.Configurations;

using System;
using System.Collections.Generic;

public class LoggerMinimumLevelConfiguration
{
    private readonly LoggerConfiguration _loggerConfiguration;
    private readonly Action<LogLevel> _levelAction;
    private readonly Action<LogLevelSwitch> _levelSwitchActin;
    private readonly Action<string, LogLevelSwitch> _overrideAction;

    public LoggerMinimumLevelConfiguration(
        LoggerConfiguration loggerConfiguration,
        Action<LogLevel> levelAction,
        Action<LogLevelSwitch> levelSwitchActin,
        Action<string, LogLevelSwitch> overrideAction)
    {
        _loggerConfiguration = loggerConfiguration;
        _levelAction = levelAction;
        _levelSwitchActin = levelSwitchActin;
        _overrideAction = overrideAction;
    }

    public LoggerConfiguration Minimum(LogLevel level)
    {
        _levelAction(level);

        return _loggerConfiguration;
    }

    public LoggerConfiguration ControlledBy(LogLevelSwitch levelSwitch)
    {
        _levelSwitchActin(levelSwitch);

        return _loggerConfiguration;
    }

    public LoggerConfiguration Verbose() => Minimum(LogLevel.Verbose);
    public LoggerConfiguration Debug() => Minimum(LogLevel.Debug);
    public LoggerConfiguration Information() => Minimum(LogLevel.Information);
    public LoggerConfiguration Warning() => Minimum(LogLevel.Warning);
    public LoggerConfiguration Error() => Minimum(LogLevel.Error);
    public LoggerConfiguration Fatal() => Minimum(LogLevel.Fatal);

    /// <summary>
    /// 重写与提供的<paramref name="sourceName"/>相同类型或命名空间的日志等级
    /// </summary>
    /// <param name="sourceName"></param>
    /// <param name="levelSwitch"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public LoggerConfiguration Override(string sourceName, LogLevelSwitch levelSwitch)
    {
        var trimmed = sourceName.Trim();
        if(trimmed.IsNullOrEmpty())
        {
            throw new ArgumentException("", nameof(sourceName));
        }

        _overrideAction(trimmed, levelSwitch);

        return _loggerConfiguration;
    }

    public LoggerConfiguration Override(string sourceName, LogLevel minimumLevel)
    {
        return Override(sourceName, new LogLevelSwitch(minimumLevel));
    }
}
