namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;
using OneI.Logable.Sinks;

public partial class LoggerConfiguration : ILoggerConfiguration, ILoggerBranchConfiguration
{
    private readonly List<Func<LoggerDelegate, LoggerDelegate>> _components;
    private readonly List<ILoggerSink> _endpoints;
    private readonly LogLevelMap _logLevelMap;

    public LoggerConfiguration()
    {
        _endpoints = new();
        _logLevelMap = new();

        Level = new LoggerLevelConfiguration(this);
        Properties = new LoggerPropertyConfiguration(this);
        Sink = new LoggerSinkBuilder(this);
        _components = new();
    }

    public ILoggerSinkConfiguration Sink { get; }
    public ILoggerLevelConfiguration Level { get; }
    public ILoggerPropertyConfiguration Properties { get; }

    public ILogger CreateLogger()
    {
        var sink = new AggregateSink(_endpoints, true);

        return new Logger(PackageMiddleware(), sink, _logLevelMap);
    }

    public ILoggerConfiguration New()
    {
        var @new = new LoggerConfiguration();

        return @new;
    }

    public virtual ILoggerConfiguration NewWhen(Func<LoggerContext, bool> condition, Action<ILoggerConfiguration> configuration)
    {
        var builder = New();

        configuration(builder);

        var branch = builder.PackageMiddleware();

        Use(new MapMiddleware(condition, branch));

        return builder;
    }

    public LoggerDelegate PackageMiddleware()
    {
        LoggerDelegate process = static context => LoggerVoid.Instance;

        for(var i = _components.Count - 1; i >= 0; i--)
        {
            process = _components[i].Invoke(process);
        }

        return process;
    }

    public virtual ILoggerConfiguration Use(ILoggerMiddleware middleware)
    {
        Use(next =>
        {
            return context => middleware.Invoke(context, next);
        });

        return this;
    }

    public virtual ILoggerConfiguration Use<TMiddleware>() where TMiddleware : ILoggerMiddleware, new()
    {
        Use(new TMiddleware());

        return this;
    }

    public virtual ILoggerConfiguration Use(Action<LoggerContext, LoggerDelegate> middleware)
    {
        Use(next => context =>
        {
            middleware.Invoke(context, next);

            return LoggerVoid.Instance;
        });

        return this;
    }

    public virtual ILoggerConfiguration Use(Action<LoggerContext> middleware)
    {
        Use(next => context =>
        {
            middleware.Invoke(context);

            return next.Invoke(context);
        });

        return this;
    }

    public virtual ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> action)
    {
        Use(new ConditionalMiddleware(condition, action));

        return this;
    }

    private ILoggerConfiguration Use(Func<LoggerDelegate, LoggerDelegate> middleware)
    {
        _components.Add(middleware);

        return this;
    }
}
