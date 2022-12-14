namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Endpoints;

public partial class LoggerConfiguration
{
    private class LoggerSinkBuilder : ILoggerSinkConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerSinkBuilder(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink endpoint)
        {
            _parent._endpoints.Add(new ConditionalSink(condition, endpoint));

            return _parent;
        }

        public ILoggerConfiguration Use(ILoggerSink endpoint)
        {
            _parent._endpoints.Add(endpoint);

            return _parent;
        }
    }
}
