namespace OneI.Logable;

using OneI.Logable.Configurations;

public partial class LoggerConfiguration
{
    private class LoggerLevelConfiguration : ILoggerLevelConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerLevelConfiguration(LoggerConfiguration parent) => _parent = parent;

        public ILoggerConfiguration Use(LogLevel minimum, LogLevel? maximum = null)
        {
            _parent._logLevelMap.Override(minimum, maximum ?? LogLevelMap.MaximumLevelDefault);

            return _parent;
        }

        public ILoggerConfiguration Override(string sourceContext, LogLevel minimum, LogLevel? maximum = null)
        {
            _ = Check.NotNullOrWhiteSpace(sourceContext);

            _parent._logLevelMap.Override(sourceContext, minimum, maximum ?? LogLevelMap.MaximumLevelDefault);

            return _parent;
        }
    }
}
