namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;
using OneI.Logable.Sinks;
/// <summary>
/// The logger configuration.
/// </summary>

public partial class LoggerConfiguration : ILoggerConfiguration, ILoggerBranchConfiguration
{
    private readonly List<Func<LoggerDelegate, LoggerDelegate>> _components;
    private readonly List<Action<LoggerContext>> _sinks;
    private readonly LogLevelMap _logLevelMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerConfiguration"/> class.
    /// </summary>
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

    /// <summary>
    /// Gets the sink.
    /// </summary>
    public ILoggerSinkConfiguration Sink { get; }
    /// <summary>
    /// Gets the level.
    /// </summary>
    public ILoggerLevelConfiguration Level { get; }
    /// <summary>
    /// Gets the properties.
    /// </summary>
    public ILoggerPropertyConfiguration Properties { get; }
    /// <summary>
    /// Gets the audit.
    /// </summary>
    public ILoggerAuditConfiguration Audit { get; }

    /// <summary>
    /// Creates the logger.
    /// </summary>
    /// <returns>An ILogger.</returns>
    public ILogger CreateLogger()
    {
        var sink = new AggregateSink(_sinks, true);

        return new Logger(PackageMiddleware(), sink, _logLevelMap);
    }

    /// <summary>
    /// Packages the middleware.
    /// </summary>
    /// <returns>A LoggerDelegate.</returns>
    public LoggerDelegate PackageMiddleware()
    {
        var process = NullMiddleware.Delegate;

        for(var i = _components.Count - 1; i >= 0; i--)
        {
            process = _components[i].Invoke(process);
        }

        return process;
    }

    /// <summary>
    /// Uses the.
    /// </summary>
    /// <param name="middleware">The middleware.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public virtual ILoggerConfiguration Use(ILoggerMiddleware middleware)
    {
        return Use(next => context => middleware.Invoke(context, next));
    }

    /// <summary>
    /// Uses the.
    /// </summary>
    /// <returns>An ILoggerConfiguration.</returns>
    public virtual ILoggerConfiguration Use<TMiddleware>() where TMiddleware : ILoggerMiddleware, new()
    {
        return Use(new TMiddleware());
    }

    /// <summary>
    /// Uses the.
    /// </summary>
    /// <param name="middleware">The middleware.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public virtual ILoggerConfiguration Use(Action<LoggerContext, LoggerDelegate> middleware)
    {
        return Use(next => context =>
        {
            middleware.Invoke(context, next);

            return next(context);
        });
    }

    /// <summary>
    /// Uses the.
    /// </summary>
    /// <param name="middleware">The middleware.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public virtual ILoggerConfiguration Use(Action<LoggerContext> middleware)
    {
        return Use(next => context =>
        {
            middleware.Invoke(context);

            return next.Invoke(context);
        });
    }

    /// <summary>
    /// Uses the when.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="middleware">The middleware.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public virtual ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerMiddleware middleware)
    {
        return Use(new ConditionalMiddleware(condition, middleware));
    }

    /// <summary>
    /// Uses the when.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public virtual ILoggerConfiguration UseWhen<TMiddleware>(Func<LoggerContext, bool> condition)
        where TMiddleware : ILoggerMiddleware, new()
    {
        return UseWhen(condition, new TMiddleware());
    }

    /// <summary>
    /// Uses the when.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="middleware">The middleware.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public virtual ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext, LoggerDelegate> middleware)
    {
        return UseWhen(condition, (context, next) =>
        {
            middleware(context, next);

            return next(context);
        });
    }

    /// <summary>
    /// Uses the when.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="middleware">The middleware.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public virtual ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> middleware)
    {
        return UseWhen(condition, (context, next) =>
        {
            middleware(context);

            return next(context);
        });
    }

    /// <summary>
    /// Uses the.
    /// </summary>
    /// <param name="middleware">The middleware.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    private ILoggerConfiguration Use(Func<LoggerDelegate, LoggerDelegate> middleware)
    {
        _components.Add(middleware);

        return this;
    }

    /// <summary>
    /// Uses the when.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="middleware">The middleware.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    private ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Func<LoggerContext, LoggerDelegate, LoggerVoid> middleware)
    {
        return Use(new ConditionalMiddleware(condition, middleware));
    }
}
