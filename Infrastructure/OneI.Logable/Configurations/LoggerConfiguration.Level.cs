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

        public LoggerConfiguration Use(LogLevelMap levelMap)
        {
            _parent._logLevelMap.Override(levelMap);

            return _parent;
        }

        public ILoggerConfiguration Minimum(LogLevel minimum)
        {
            _parent._logLevelMap.Minimum(minimum);

            return _parent;
        }

        public ILoggerConfiguration Verbose()
        {
            Minimum(LogLevel.Verbose);

            return _parent;
        }

        public ILoggerConfiguration Debug()
        {
            Minimum(LogLevel.Debug);

            return _parent;
        }

        public ILoggerConfiguration Information()
        {
            Minimum(LogLevel.Information);

            return _parent;
        }

        public ILoggerConfiguration Warning()
        {
            Minimum(LogLevel.Warning);

            return _parent;
        }

        public ILoggerConfiguration Error()
        {
            Minimum(LogLevel.Error);

            return _parent;
        }

        public ILoggerConfiguration Fatal()
        {
            Minimum(LogLevel.Fatal);

            return _parent;
        }

        public ILoggerConfiguration Maximum(LogLevel maximum)
        {
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
