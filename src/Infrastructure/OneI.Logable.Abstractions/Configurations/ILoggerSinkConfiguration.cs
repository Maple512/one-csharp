namespace OneI.Logable;

public interface ILoggerSinkConfiguration
{
    ILoggerConfiguration Use(ILoggerSink sink);

    ILoggerConfiguration Use<TSink>() where TSink : ILoggerSink, new()
        => Use(new TSink());

    ILoggerConfiguration Use(Action<LoggerContext> sink);

    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink sink);

    ILoggerConfiguration UseWhen<TSink>(Func<LoggerContext, bool> condition) where TSink : ILoggerSink, new()
        => UseWhen(condition, new TSink());

    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> sink);

    ILoggerConfiguration Logger(ILogger logger, bool autoDispose = false);

    ILoggerConfiguration LoggerWhen(Func<LoggerContext, bool> condition, ILogger logger, bool autoDispose = false);

    ILoggerConfiguration Logger(Action<ILoggerConfiguration> configure, bool autoDispose = false);

    ILoggerConfiguration LoggerWhen(Func<LoggerContext, bool> condition, Action<ILoggerConfiguration> configure, bool autoDispose = false);
}
