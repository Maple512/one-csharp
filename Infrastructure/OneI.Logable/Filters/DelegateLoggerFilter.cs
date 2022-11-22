namespace OneI.Logable.Filters;

using System;

public class DelegateLoggerFilter : ILoggerFilter
{
    private readonly Func<LoggerContext, bool> _filter;

    public DelegateLoggerFilter(Func<LoggerContext, bool> filter)
    {
        _filter = filter;
    }

    public bool IsEnabled(LoggerContext context)
    {
        return _filter(context);
    }
}
