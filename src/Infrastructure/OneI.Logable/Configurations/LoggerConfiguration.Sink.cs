namespace OneI.Logable;

using OneI.Logable.Configurations;

public partial class LoggerConfiguration
{
    private class LoggerSinkConfiguration : ILoggerSinkConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerSinkConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration Use(ILoggerSink sink)
        {
            _parent._sinks.Add(sink);

            return _parent;
        }
    }
}
