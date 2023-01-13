namespace OneI.Logable.Configurations;

using OneI.Logable;
using OneI.Logable.Sinks;

public interface ILoggerSinkConfiguration
{
    ILoggerConfiguration Use(ILoggerSink sink);

    ILoggerConfiguration Use(Action<LoggerContext> sink) => Use(new ActionSink(sink));

    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink sink)
        => Use(new ConditionalSink(condition, sink));

    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> sink)
        => Use(new ConditionalSink(condition, new ActionSink(sink)));
}
