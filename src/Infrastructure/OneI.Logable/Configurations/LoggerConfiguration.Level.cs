namespace OneI.Logable;

using OneI.Logable.Configurations;

public partial class LoggerConfiguration
{
    private class LoggerLevelConfiguration : ILoggerLevelConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerLevelConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public LoggerConfiguration Use(LogLevel minimum, LogLevel? maximum = null)
        {
            _parent._logLevelMap.Minimum(minimum);
            _parent._logLevelMap.Maximum(maximum);

            return _parent;
        }

        public ILoggerConfiguration Override(string sourceContext, LogLevel minimum, LogLevel? maximum = null)
        {
            if(sourceContext.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(sourceContext));
            }

            _parent._logLevelMap.Override(sourceContext, minimum, maximum);

            return _parent;
        }
    }
}
