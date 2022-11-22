namespace OneI.Logable.Sinks;

using System;
using System.Collections.Generic;
using OneI.Logable.Filters;

public class FilterSink : ILoggerSink
{
    private readonly IReadOnlyList<ILoggerFilter> _filters;
    private readonly ILoggerSink _next;
    private readonly bool _propagateExceptions;

    public FilterSink(
        IReadOnlyList<ILoggerFilter> filters,
        ILoggerSink next,
        bool propagateExceptions)
    {
        _filters = filters;
        _next = next;
        _propagateExceptions = propagateExceptions;
    }

    public void Emit(LoggerContext context)
    {
        try
        {
            foreach(var filter in _filters)
            {
                if(filter.IsEnabled(context) == false)
                {
                    return;
                }
            }

            _next.Emit(context);
        }
        catch(Exception)
        {
            if(_propagateExceptions)
            {
                throw;
            }
        }
    }
}
