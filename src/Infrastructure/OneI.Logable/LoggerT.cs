namespace OneI.Logable;

using OneI.Logable.Internal;
using OneI.Logable.Templates;

[StackTraceHidden]
public class Logger<TSource> : Logger, ILogger<TSource>, ILogger
{
    internal Logger(
        ILoggerMiddleware[] middleware,
        ILoggerSink[] sinks,
        LogLevelMap levelMap,
        TemplateProvider templateProvider) : base(middleware, sinks, levelMap, templateProvider)
    {
    }
}
