namespace OneI.Logable.Configurations;

using OneI.Logable.Middlewares;
using OneI.Logable.Sinks;

public partial class LoggerConfiguration : ILoggerConfiguration, ILoggerBranchConfiguration
{
    private readonly List<Func<LoggerDelegate, LoggerDelegate>> _components;
    private readonly List<Action<LoggerContext>> _sinks;
    private readonly LogLevelMap _logLevelMap;

    public LoggerConfiguration()
    {
        _sinks = new();
        _logLevelMap = new();
        _components = new();

        Level = new LoggerLevelConfiguration(this);
        Properties = new LoggerPropertyConfiguration(this);
        Sink = new LoggerSinkBuilder(this);
        Audit = new LoggerAuditConfiguration(this);
    }

    public ILoggerSinkConfiguration Sink { get; }
    public ILoggerLevelConfiguration Level { get; }
    public ILoggerPropertyConfiguration Properties { get; }
    public ILoggerAuditConfiguration Audit { get; }

    public ILogger CreateLogger()
    {
        var sink = new AggregateSink(_sinks, true);

        return new Logger(PackageMiddleware(), sink, _logLevelMap);
    }

    public LoggerDelegate PackageMiddleware()
    {
        var process = NullMiddleware.Delegate;

        for(var i = _components.Count - 1; i >= 0; i--)
        {
            process = _components[i].Invoke(process);
        }

        return process;
    }

    public virtual ILoggerConfiguration Use(ILoggerMiddleware middleware)
    {
        return Use(next => context => middleware.Invoke(context, next));
    }

    public virtual ILoggerConfiguration Use<TMiddleware>() where TMiddleware : ILoggerMiddleware, new()
    {
        return Use(new TMiddleware());
    }

    public virtual ILoggerConfiguration Use(Action<LoggerContext, LoggerDelegate> middleware)
    {
        return Use(next => context =>
        {
            middleware.Invoke(context, next);

            return LoggerVoid.Instance;
        });
    }

    public virtual ILoggerConfiguration Use(Action<LoggerContext> middleware)
    {
        return Use(next => context =>
        {
            middleware.Invoke(context);

            return next.Invoke(context);
        });
    }

    public virtual ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerMiddleware middleware)
    {
        return Use(new ConditionalMiddleware(condition, middleware));
    }

    public virtual ILoggerConfiguration UseWhen<TMiddleware>(Func<LoggerContext, bool> condition)
        where TMiddleware : ILoggerMiddleware, new()
    {
        return UseWhen(condition, new TMiddleware());
    }

    public virtual ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext, LoggerDelegate> middleware)
    {
        return UseWhen(condition, (context, next) =>
        {
            middleware(context, next);

            return LoggerVoid.Instance;
        });
    }

    public virtual ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> middleware)
    {
        return UseWhen(condition, (context, next) =>
        {
            middleware(context);

            return LoggerVoid.Instance;
        });
    }

    private ILoggerConfiguration Use(Func<LoggerDelegate, LoggerDelegate> middleware)
    {
        _components.Add(middleware);

        return this;
    }

    private ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Func<LoggerContext, LoggerDelegate, LoggerVoid> middleware)
    {
        return Use(new ConditionalMiddleware(condition, middleware));
    }
}
