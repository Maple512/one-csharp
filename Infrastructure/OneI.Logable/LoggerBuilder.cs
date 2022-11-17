namespace OneI.Logable;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LoggerBuilder : ILoggerBuilder
{
    private readonly List<Func<ILoggerContext,Task<bool>>> _filters = new();

    private readonly List<LogDelegate> _branchs = new();

    public ILoggerBuilder Filter(Func<ILoggerContext, Task<bool>> filter)
    {
        _filters.Add(filter);

        return this;
    }

    public ILoggerBuilder Branch(LogDelegate branch)
    {
        _branchs.Add(branch);

        return this;
    }

    public ILogger Build()
    {
        return new Logger(_filters,_branchs);
    }
}
