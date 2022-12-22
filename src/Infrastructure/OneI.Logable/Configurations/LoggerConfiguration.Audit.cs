namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;
/// <summary>
/// The logger configuration.
/// </summary>

public partial class LoggerConfiguration
{
    /// <summary>
    /// The logger audit configuration.
    /// </summary>
    private class LoggerAuditConfiguration : ILoggerAuditConfiguration
    {
        private readonly ILoggerConfiguration _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerAuditConfiguration"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public LoggerAuditConfiguration(ILoggerConfiguration parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Attaches the.
        /// </summary>
        /// <param name="audit">The audit.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Attach(AuditDelegate audit)
        {
            return _parent.Use(new AuditMiddleware(c => audit(c)));
        }

        /// <summary>
        /// Attaches the.
        /// </summary>
        /// <param name="audit">The audit.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration Attach(Action<LoggerContext> audit)
        {
            return _parent.Use(new AuditMiddleware(audit));
        }

        /// <summary>
        /// Attaches the when.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="audit">The audit.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, AuditDelegate audit)
        {
            return _parent.Use(new ConditionalMiddleware(condition, new AuditMiddleware(c => audit(c))));
        }

        /// <summary>
        /// Attaches the when.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="audit">The audit.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> audit)
        {
            return _parent.Use(new ConditionalMiddleware(condition, new AuditMiddleware(audit)));
        }
    }
}
