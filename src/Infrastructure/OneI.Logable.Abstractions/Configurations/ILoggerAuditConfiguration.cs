namespace OneI.Logable;

public interface ILoggerAuditConfiguration
{
    ILoggerConfiguration Attach(ILoggerSink auditor);

    ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, ILoggerSink auditor);

    ILoggerConfiguration Attach(Action<LoggerContext> auditor);

    ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> auditor);

    ILoggerConfiguration Attach<TSink>()
        where TSink : ILoggerSink, new()
        => Attach(new TSink());

    ILoggerConfiguration AttachWhen<TSink>(Func<LoggerContext, bool> condition)
        where TSink : ILoggerSink, new();

    ILoggerConfiguration Attach(ILogger logger, bool autoDispose = false);

    ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, ILogger logger, bool autoDispose = false);

    ILoggerConfiguration Attach<TLogger>(bool autoDispose = false)
        where TLogger : ILogger, new();

    ILoggerConfiguration AttachWhen<TLogger>(Func<LoggerContext, bool> condition, bool autoDispose = false)
        where TLogger : ILogger, new();
}
