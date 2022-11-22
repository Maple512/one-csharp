namespace OneI.Logable.Configurations;

using System;
using OneI.Logable.Filters;

public class LoggerFilterConfiguration
{
    private readonly LoggerConfiguration _loggerConfiguration;
    private readonly Action<ILoggerFilter> _filterAction;

    public LoggerFilterConfiguration(LoggerConfiguration loggerConfiguration, Action<ILoggerFilter> filterAction)
    {
        _loggerConfiguration = loggerConfiguration;
        _filterAction = filterAction;
    }

    public LoggerConfiguration With(params ILoggerFilter[] filters)
    {
        foreach(var filter in filters)
        {
            _filterAction(filter);
        }

        return _loggerConfiguration;
    }

    public LoggerConfiguration With<TFilter>()
        where TFilter : ILoggerFilter, new()
    {
        return With(new TFilter());
    }

    public LoggerConfiguration ByExcluding(Func<LoggerContext, bool> exclustionPredicate)
    {
        return With(new DelegateLoggerFilter(context => exclustionPredicate(context) == false));
    }

    public LoggerConfiguration ByIncludingOnly(Func<LoggerContext, bool> predicate)
    {
        return With(new DelegateLoggerFilter(predicate));
    }
}
