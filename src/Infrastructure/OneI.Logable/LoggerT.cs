namespace OneI.Logable;

using OneI.Logable.Templates;

[StackTraceHidden]
public class Logger<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T> : Logger, ILogger<T>, ILogger
{
    internal Logger(
        ILoggerMiddleware[] middleware,
        ILoggerSink[] sinks,
        LogLevelMap levelMap,
        TemplateProvider templateProvider) : base(middleware, sinks, levelMap, templateProvider)
    {
    }
}
