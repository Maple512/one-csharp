namespace OneI.Logable;

using OneI.Logable.Sinks;

public partial class LoggerConfiguration
{
    internal class LoggerSinkConfiguration : ILoggerSinkConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerSinkConfiguration(LoggerConfiguration parent) => _parent = parent;

        public ILoggerConfiguration Use(ILoggerSink sink)
        {
            _parent._sinks.Add(sink);

            return _parent;
        }

        public ILoggerConfiguration Use(Action<LoggerContext> sink) => Use(new ActionSink(sink));

        public ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink sink)
            => Use(new ConditionalSink(condition, sink));

        public ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> sink)
            => Use(new ConditionalSink(condition, new ActionSink(sink)));

        public ILoggerConfiguration Logger(ILogger logger, bool autoDispose = false)
            => Use(new SecondaryLoggerSink(logger, autoDispose));

        public ILoggerConfiguration LoggerWhen(Func<LoggerContext, bool> condition, ILogger logger, bool autoDispose = false)
            => UseWhen(condition, new SecondaryLoggerSink(logger, autoDispose));

        public ILoggerConfiguration Logger(Action<ILoggerConfiguration> configure, bool autoDispose = false)
        {
            var configuration = new LoggerConfiguration();
            configure(configuration);
            var logger = configuration.CreateLogger();

            return Logger(logger, autoDispose);
        }

        public ILoggerConfiguration LoggerWhen(Func<LoggerContext, bool> condition
                                        , Action<ILoggerConfiguration> configure
                                        , bool autoDispose = false)
        {
            var configuration = new LoggerConfiguration();
            configure(configuration);
            var logger = configuration.CreateLogger();

            return LoggerWhen(condition, logger, autoDispose);
        }
    }
}
