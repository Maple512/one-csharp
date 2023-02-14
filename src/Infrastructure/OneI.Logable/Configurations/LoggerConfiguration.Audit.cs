namespace OneI.Logable;

using OneI.Logable.Sinks;

public partial class LoggerConfiguration
{
    private class AuditConfiguration : ILoggerAuditConfiguration
    {
        private readonly ILoggerConfiguration _parent;

        public AuditConfiguration(ILoggerConfiguration parent) => _parent = parent;

        public ILoggerConfiguration Attach(ILoggerSink auditor)
        {
            _ = _parent.Sink.Use(auditor);

            return _parent;
        }

        public ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, ILoggerSink auditor)
            => Attach(new ConditionalSink(condition, auditor));

        public ILoggerConfiguration Attach(Action<LoggerContext> auditor) => Attach(new ActionSink(auditor));

        public ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> auditor)
            => Attach(new ConditionalSink(condition, new ActionSink(auditor)));

        public ILoggerConfiguration AttachWhen<TSink>(Func<LoggerContext, bool> condition)
            where TSink : ILoggerSink, new()
            => Attach(new ConditionalSink(condition, new TSink()));

        public ILoggerConfiguration Attach(ILogger logger, bool autoDispose = false)
            => Attach(new SecondaryLoggerSink(logger, autoDispose));

        public ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, ILogger logger, bool autoDispose = false)
            => Attach(new ConditionalSink(condition, new SecondaryLoggerSink(logger, autoDispose)));

        public ILoggerConfiguration Attach<TLogger>(bool autoDispose = false)
            where TLogger : ILogger, new()
            => Attach(new TLogger(), autoDispose);

        public ILoggerConfiguration AttachWhen<TLogger>(Func<LoggerContext, bool> condition, bool autoDispose = false)
            where TLogger : ILogger, new()
            => AttachWhen(condition, new TLogger(), autoDispose);
    }
}
