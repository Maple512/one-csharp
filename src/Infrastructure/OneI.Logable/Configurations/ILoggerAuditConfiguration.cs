namespace OneI.Logable.Configurations;

using OneI.Logable.Sinks;

public interface ILoggerAuditConfiguration
{
    ILoggerConfiguration Attact(ILoggerSink auditor);

    ILoggerConfiguration AttactWhen(Func<LoggerContext, bool> condition, ILoggerSink auditor)
        => Attact(new ConditionalSink(condition, auditor));

    ILoggerConfiguration Attact(Action<LoggerContext> auditor)
        => Attact(new ActionSink(auditor));

    ILoggerConfiguration AttactWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> auditor)
        => Attact(new ConditionalSink(condition, new ActionSink(auditor)));

    ILoggerConfiguration Attact<TSink>() where TSink : ILoggerSink, new()
        => Attact(new TSink());

    ILoggerConfiguration AttactWhen<TSink>(Func<LoggerContext, bool> condition)
         where TSink : ILoggerSink, new()
        => Attact(new ConditionalSink(condition, new TSink()));

    ILoggerConfiguration Attact(ILogger logger, bool autoDispose = false)
        => Attact(new SecondaryLoggerSink(logger, autoDispose));

    ILoggerConfiguration AttactWhen(Func<LoggerContext, bool> condition, ILogger logger, bool autoDispose = false)
        => Attact(new ConditionalSink(condition, new SecondaryLoggerSink(logger, autoDispose)));

    ILoggerConfiguration Attact<TLogger>(bool autoDispose = false)
        where TLogger : ILogger, new()
        => Attact(new TLogger(), autoDispose);

    ILoggerConfiguration AttactWhen<TLogger>(Func<LoggerContext, bool> condition, bool autoDispose = false)
        where TLogger : ILogger, new()
        => AttactWhen(condition, new TLogger(), autoDispose);
}
