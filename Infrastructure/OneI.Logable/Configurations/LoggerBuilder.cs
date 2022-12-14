namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Endpoints;
using OneI.Logable.Middlewares;

public partial class LoggerBuilder : ILoggerBuilder, ILoggerBranchBuilder
{
    private readonly List<Func<LoggerDelegate, LoggerDelegate>> _components;
    private readonly List<ILoggerEndpoint> _endpoints;
    private readonly LogLevelMap _logLevelMap;

    public LoggerBuilder()
    {
        _endpoints = new();
        _logLevelMap = new();

        Level = new LoggerLevelBuilder(this);
        Properties = new LoggerPropertyBuilder(this);
        Endpoint = new LoggerEndpointBuilder(this);
        _components = new();
    }

    public ILoggerEndpointBuilder Endpoint { get; }
    public ILoggerLevelBuilder Level { get; }
    public ILoggerPropertyBuilder Properties { get; }

    public ILogger CreateLogger()
    {
        var endpoint = new AggregateEndpoint(_endpoints, true);

        return new Logger(PackageMiddleware(), endpoint, _logLevelMap);
    }

    public ILoggerBuilder New()
    {
        var @new = new LoggerBuilder();

        return @new;
    }

    public virtual ILoggerBuilder NewWhen(Func<LoggerContext, bool> condition, Action<ILoggerBuilder> configuration)
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

    public virtual ILoggerBuilder Use(ILoggerMiddleware middleware)
    {
        Use(next =>
        {
            return context => middleware.Invoke(context, next);
        });

        return this;
    }

    public virtual ILoggerBuilder Use<TMiddleware>() where TMiddleware : ILoggerMiddleware, new()
    {
        Use(new TMiddleware());

        return this;
    }

    public virtual ILoggerBuilder Use(Action<LoggerContext, LoggerDelegate> middleware)
    {
        Use(next => context =>
        {
            middleware.Invoke(context, next);

            return LoggerVoid.Instance;
        });

        return this;
    }

    public virtual ILoggerBuilder Use(Action<LoggerContext> middleware)
    {
        Use(next => context =>
        {
            middleware.Invoke(context);

            return next.Invoke(context);
        });

        return this;
    }

    public virtual ILoggerBuilder UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> action)
    {
        Use(new ConditionalMiddleware(condition, action));

        return this;
    }

    private ILoggerBuilder Use(Func<LoggerDelegate, LoggerDelegate> middleware)
    {
        _components.Add(middleware);

        return this;
    }
}
