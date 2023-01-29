namespace OneI.Logable;

using Configurations;
using Middlewares;
using Templates;

public partial class LoggerConfiguration : ILoggerConfiguration, ILoggerPipelineConfiguration
{
    public const string DefaultTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}";

    private readonly List<ILoggerMiddleware> _middlewares;
    private readonly List<TemplateItem> _templateProviders;
    private ReadOnlyMemory<char> _defaultTemplate;
    private readonly List<ILoggerSink> _sinks;
    private readonly LogLevelMap _logLevelMap;

    public LoggerConfiguration()
    {
        _templateProviders = new();
        _sinks = new();
        _logLevelMap = new();
        _middlewares = new();

        Level = new LoggerLevelConfiguration(this);
        Properties = new LoggerPropertyConfiguration(this);
        Sink = new LoggerSinkConfiguration(this);
        Template = new TemplateConfiguration(this);
        Audit = new AuditConfiguration(this);
    }

    public ILoggerTemplateConfiguration Template { get; }

    public ILoggerSinkConfiguration Sink { get; }

    public ILoggerLevelConfiguration Level { get; }

    public ILoggerPropertyConfiguration Properties { get; }

    public ILoggerAuditConfiguration Audit { get; }

    public virtual ILoggerConfiguration Use(ILoggerMiddleware middleware)
    {
        _middlewares.Add(middleware);

        return this;
    }

    public ILoggerConfiguration Use<TMiddleware>() where TMiddleware : ILoggerMiddleware, new()
    {
        return Use(new TMiddleware());
    }

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration Use(Action<LoggerMessageContext> middleware)
    {
        return Use(new ActionMiddleware(middleware));
    }

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, ILoggerMiddleware middleware)
    {
        return Use(new ConditionalMiddleware(condition, middleware));
    }

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration UseWhen<TMiddleware>(Func<LoggerMessageContext, bool> condition)
        where TMiddleware : ILoggerMiddleware, new()
    {
        return UseWhen(condition, new TMiddleware());
    }

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, Action<LoggerMessageContext> middleware)
    {
        return UseWhen(condition, new ActionMiddleware(middleware));
    }

    public ILogger CreateLogger()
    {
        var defaultTemplate = _defaultTemplate;
        if(defaultTemplate.IsEmpty)
        {
            defaultTemplate = DefaultTemplate.AsMemory();
        }

        var templateSelector = new TemplateProvider(defaultTemplate, _templateProviders.ToArray());

        return new Logger(_middlewares.ToArray(), _sinks.ToArray(), _logLevelMap, templateSelector);
    }

    internal ILogger CreateWithLogger(Logger logger)
    {
        var defaultTemplate = _defaultTemplate;
        if(defaultTemplate.IsEmpty)
        {
            defaultTemplate = DefaultTemplate.AsMemory();
        }

        var loggerMiddlwares = logger._middlewares ?? Array.Empty<ILoggerMiddleware>();

        var middlewareCount = loggerMiddlwares.Length + _middlewares.Count;
        var middlewares = middlewareCount == 0 ? Array.Empty<ILoggerMiddleware>() : new ILoggerMiddleware[middlewareCount];

        if(loggerMiddlwares.Length > 0)
        {
            loggerMiddlwares.CopyTo(middlewares, 0);
        }

        if(_middlewares.Count > 0)
        {
            _middlewares.CopyTo(middlewares, loggerMiddlwares.Length);
        }

        var sinkCount = _sinks.Count + logger._sinks.Length;
        var sinks = sinkCount == 0 ? Array.Empty<ILoggerSink>() : new ILoggerSink[sinkCount];

        if(logger._sinks.Length > 0)
        {
            logger._sinks.CopyTo(sinks, 0);
        }

        if(_sinks.Count > 0)
        {
            _sinks.CopyTo(sinks, logger._sinks.Length);
        }

        var templateProvider = new TemplateProvider(defaultTemplate, _templateProviders.ToArray());

        return new Logger(middlewares, sinks, _logLevelMap, templateProvider);
    }
}
