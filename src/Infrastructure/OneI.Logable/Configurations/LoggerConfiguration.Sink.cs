namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Sinks;

public partial class LoggerConfiguration
{
    private class LoggerSinkBuilder : ILoggerSinkConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerSinkBuilder(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration Use(ILoggerSink sink)
        {
            _parent._sinks.Add(context => sink.Invoke(context));

            return _parent;
        }

        public ILoggerConfiguration Use(Action<LoggerContext> sink)
        {
            _parent._sinks.Add(sink);

            return _parent;
        }

        public ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink sink)
        {
            return Use(new ConditionalSink(condition, context => sink.Invoke(context)));
        }

        public ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> sink)
        {
            return Use(new ConditionalSink(condition, sink));
        }
    }
}
