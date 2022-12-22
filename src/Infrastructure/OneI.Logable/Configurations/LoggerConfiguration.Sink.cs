namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Sinks;
/// <summary>
/// The logger configuration.
/// </summary>

public partial class LoggerConfiguration
{
    /// <summary>
    /// The logger sink builder.
    /// </summary>
    private class LoggerSinkBuilder : ILoggerSinkConfiguration
    {
        private readonly LoggerConfiguration _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerSinkBuilder"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public LoggerSinkBuilder(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Uses the.
        /// </summary>
        /// <param name="sink">The sink.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Use(ILoggerSink sink)
        {
            _parent._sinks.Add(context => sink.Invoke(context));

            return _parent;
        }

        /// <summary>
        /// Uses the.
        /// </summary>
        /// <param name="sink">The sink.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Use(Action<LoggerContext> sink)
        {
            _parent._sinks.Add(sink);

            return _parent;
        }

        /// <summary>
        /// Uses the when.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="sink">The sink.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink sink)
        {
            return Use(new ConditionalSink(condition, context => sink.Invoke(context)));
        }

        /// <summary>
        /// Uses the when.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="sink">The sink.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> sink)
        {
            return Use(new ConditionalSink(condition, sink));
        }
    }
}
