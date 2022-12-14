namespace OneI.Logable;

public partial class LoggerBuilder
{
    private class LoggerLevelBuilder : ILoggerLevelBuilder
    {
        private readonly LoggerBuilder _parent;

        public LoggerLevelBuilder(LoggerBuilder parent)
        {
            _parent = parent;
        }

        public ILoggerBuilder Minimum(LogLevel minimum)
        {
            _parent._logLevelMap.Minimum(minimum);

            return _parent;
        }

        public ILoggerBuilder Verbose()
        {
            Minimum(LogLevel.Verbose);

            return _parent;
        }

        public ILoggerBuilder Debug()
        {
            Minimum(LogLevel.Debug);

            return _parent;
        }

        public ILoggerBuilder Information()
        {
            Minimum(LogLevel.Information);

            return _parent;
        }

        public ILoggerBuilder Warning()
        {
            Minimum(LogLevel.Warning);

            return _parent;
        }

        public ILoggerBuilder Error()
        {
            Minimum(LogLevel.Error);

            return _parent;
        }

        public ILoggerBuilder Fatal()
        {
            Minimum(LogLevel.Fatal);

            return _parent;
        }

        public ILoggerBuilder Maximum(LogLevel maximum)
        {
            _parent._logLevelMap.Maximum(maximum);

            return _parent;
        }

        public ILoggerBuilder Override(string sourceContext, LogLevel minimum, LogLevel? maximum = null)
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
