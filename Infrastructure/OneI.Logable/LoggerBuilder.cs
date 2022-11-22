namespace OneI.Logable.Internal;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OneI.Logable;

public class LoggerBuilder
{
    private readonly List<Func<LoggerContext, Task<bool>>> _filters = new();

    private readonly List<LogDelegate> _branchs = new();

    public LoggerBuilder Filter(Func<LoggerContext, Task<bool>> filter)
    {
        _filters.Add(filter);

        return this;
    }

    public LoggerBuilder Branch(LogDelegate branch)
    {
        _branchs.Add(branch);

        return this;
    }

    public Logger Build()
    {
        throw new Exception();
        //return new Logger(_filters, _branchs);
    }
}
