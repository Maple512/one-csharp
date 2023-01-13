namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;
using OneI.Logable.Sinks;
using OneI.Logable.Templatizations;

public partial class LoggerConfiguration : ILoggerConfiguration, ILoggerPipelineConfiguration
{
    public const string DefaultTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}";

    private readonly List<ILoggerMiddleware> _components;
    private readonly List<ILoggerMiddleware> _lastComponents;
    private readonly List<Func<LoggerMessageContext, string?>> _templateTokens;
    private readonly string _defaultTemplate;
    private readonly List<ILoggerSink> _sinks;
    private readonly LogLevelMap _logLevelMap;

    public LoggerConfiguration(string template = DefaultTemplate)
    {
        _defaultTemplate = Check.NotNullOrEmpty(template);
        _templateTokens = new(5);
        _sinks = new();
        _logLevelMap = new();
        _components = new();
        _lastComponents = new();

        Level = new LoggerLevelConfiguration(this);
        Properties = new LoggerPropertyConfiguration(this);
        Sink = new LoggerSinkConfiguration(this);
        Template = new TemplateConfiguration(this);
    }

    public ILoggerTemplateConfiguration Template { get; }

    public ILoggerSinkConfiguration Sink { get; }

    public ILoggerLevelConfiguration Level { get; }

    public ILoggerPropertyConfiguration Properties { get; }

    public virtual ILoggerConfiguration Use(ILoggerMiddleware middleware)
    {
        _components.Add(middleware);

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
        var sink = new AggregateSink(_sinks, true);

        var middlewares = new ILoggerMiddleware[_components.Count + _lastComponents.Count];

        _components.CopyTo(middlewares);
        _lastComponents.CopyTo(middlewares, _components.Count);

        var templateSelector = new TemplateSelector(_defaultTemplate, _templateTokens);

        return new Logger(new AggregateMiddleware(middlewares), sink, _logLevelMap, templateSelector);
    }
}
