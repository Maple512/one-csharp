namespace OneI.Logable.Configurations;

public interface ILoggerSinkConfiguration
{
    ILoggerConfiguration Use(ILoggerSink sink);

    ILoggerConfiguration Use(Action<LoggerContext> sink);

    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink sink);

    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> sink);
}
