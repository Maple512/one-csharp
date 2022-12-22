namespace OneI.Logable;

using OneI.Logable.Configurations;
/// <summary>
/// The logger configuration.
/// </summary>

public partial class LoggerConfiguration
{
    /// <summary>
    /// The logger level configuration.
    /// </summary>
    private class LoggerLevelConfiguration : ILoggerLevelConfiguration
    {
        private readonly LoggerConfiguration _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerLevelConfiguration"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public LoggerLevelConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Uses the.
        /// </summary>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <returns>A LoggerConfiguration.</returns>
        public LoggerConfiguration Use(LogLevel minimum, LogLevel? maximum = null)
        {
            _parent._logLevelMap.Minimum(minimum);
            _parent._logLevelMap.Maximum(maximum);

            return _parent;
        }

        /// <summary>
        /// Minimums the.
        /// </summary>
        /// <param name="minimum">The minimum.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Minimum(LogLevel minimum)
        {
            _parent._logLevelMap.Minimum(minimum);

            return _parent;
        }

        /// <summary>
        /// Verboses the.
        /// </summary>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Verbose()
        {
            Minimum(LogLevel.Verbose);

            return _parent;
        }

        /// <summary>
        /// Debugs the.
        /// </summary>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Debug()
        {
            Minimum(LogLevel.Debug);

            return _parent;
        }

        /// <summary>
        /// Information the.
        /// </summary>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Information()
        {
            Minimum(LogLevel.Information);

            return _parent;
        }

        /// <summary>
        /// Warnings the.
        /// </summary>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Warning()
        {
            Minimum(LogLevel.Warning);

            return _parent;
        }

        /// <summary>
        /// Errors the.
        /// </summary>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Error()
        {
            Minimum(LogLevel.Error);

            return _parent;
        }

        /// <summary>
        /// Fatals the.
        /// </summary>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Fatal()
        {
            Minimum(LogLevel.Fatal);

            return _parent;
        }

        /// <summary>
        /// Maximums the.
        /// </summary>
        /// <param name="maximum">The maximum.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Maximum(LogLevel maximum)
        {
            _parent._logLevelMap.Maximum(maximum);

            return _parent;
        }

        /// <summary>
        /// Overrides the.
        /// </summary>
        /// <param name="sourceContext">The source context.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <returns>An ILoggerConfiguration.</returns>
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
