namespace OneI.Logable;

using OneI.Logable.Internal;
using OneI.Logable.Middlewares;
using OneI.Logable.Templates;

public partial class LoggerConfiguration : ILoggerConfiguration, ILoggerPipelineConfiguration
{
    public const string DefaultTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss}[{Level}]{Message}{NewLine}";
    private readonly TemplateItem _defaultTemplate;
    private readonly LogLevelMap _logLevelMap;

    private readonly List<ILoggerMiddleware> _middlewares;
    private readonly List<ILoggerSink> _sinks;
    private readonly List<TemplateItem> _templateProviders;

    /// <param name="template">日志默认模板，默认：<c>{Timestamp:yyyy-MM-dd HH:mm:ss}[{Level}]{Message}{NewLine}</c></param>
    public LoggerConfiguration(string template = DefaultTemplate)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(template);

        _defaultTemplate = new TemplateItem(null, template.AsMemory());
        _templateProviders = new List<TemplateItem>();
        _sinks = new List<ILoggerSink>();
        _logLevelMap = new LogLevelMap();
        _middlewares = new List<ILoggerMiddleware>();

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

    #region Pipeline

    public virtual ILoggerConfiguration With(ILoggerMiddleware middleware)
    {
        _middlewares.Add(middleware);

        return this;
    }

    public ILoggerConfiguration With(Action<LoggerMessageContext> middleware)
        => With(new ActionMiddleware(middleware));

    public ILoggerConfiguration WithWhen(Func<LoggerMessageContext, bool> condition, ILoggerMiddleware middleware)
        => With(new ConditionalMiddleware(condition, middleware));

    public ILoggerConfiguration WithWhen(Func<LoggerMessageContext, bool> condition, Action<LoggerMessageContext> middleware)
        => WithWhen(condition, new ActionMiddleware(middleware));

    #endregion

    public ILogger CreateLogger()
    {
        var templateSelector = new TemplateProvider(_defaultTemplate, _templateProviders.ToArray());

        return new Logger(_middlewares.ToArray(), _sinks.ToArray(), _logLevelMap, templateSelector);
    }

    internal ILogger CreateWithLogger(Logger logger)
    {
        var defaultTemplate = _defaultTemplate;
        var loggerMiddlwares = logger._middlewares ?? Array.Empty<ILoggerMiddleware>();

        var middlewareCount = loggerMiddlwares.Length + _middlewares.Count;
        var middlewares = middlewareCount == 0
            ? Array.Empty<ILoggerMiddleware>()
            : new ILoggerMiddleware[middlewareCount];

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
