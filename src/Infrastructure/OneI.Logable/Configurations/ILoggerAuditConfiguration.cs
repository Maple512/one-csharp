namespace OneI.Logable.Configurations;

using Sinks;

public interface ILoggerAuditConfiguration
{
    ILoggerConfiguration Attach(ILoggerSink auditor);

    ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, ILoggerSink auditor)
        => Attach(new ConditionalSink(condition, auditor));

    ILoggerConfiguration Attach(Action<LoggerContext> auditor)
        => Attach(new ActionSink(auditor));

    ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> auditor)
        => Attach(new ConditionalSink(condition, new ActionSink(auditor)));

    ILoggerConfiguration Attach<TSink>() where TSink : ILoggerSink, new()
        => Attach(new TSink());

    ILoggerConfiguration AttachWhen<TSink>(Func<LoggerContext, bool> condition)
         where TSink : ILoggerSink, new()
        => Attach(new ConditionalSink(condition, new TSink()));

    ILoggerConfiguration Attach(ILogger logger, bool autoDispose = false)
        => Attach(new SecondaryLoggerSink(logger, autoDispose));

    ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, ILogger logger, bool autoDispose = false)
        => Attach(new ConditionalSink(condition, new SecondaryLoggerSink(logger, autoDispose)));

    ILoggerConfiguration Attach<TLogger>(bool autoDispose = false)
        where TLogger : ILogger, new()
        => Attach(new TLogger(), autoDispose);

    ILoggerConfiguration AttachWhen<TLogger>(Func<LoggerContext, bool> condition, bool autoDispose = false)
        where TLogger : ILogger, new()
        => AttachWhen(condition, new TLogger(), autoDispose);
}
