namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;

public partial class LoggerConfiguration
{
    private class LoggerAuditConfiguration : ILoggerAuditConfiguration
    {
        private readonly ILoggerConfiguration _parent;

        public LoggerAuditConfiguration(ILoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration Attach(AuditDelegate audit)
        {
            return _parent.Use(new AuditMiddleware(c => audit(c)));
        }

        public ILoggerConfiguration Attach(Action<LoggerContext> audit)
        {
            return _parent.Use(new AuditMiddleware(audit));
        }

        public ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, AuditDelegate audit)
        {
            return _parent.Use(new ConditionalMiddleware(condition, new AuditMiddleware(c => audit(c))));
        }

        public ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> audit)
        {
            return _parent.Use(new ConditionalMiddleware(condition, new AuditMiddleware(audit)));
        }
    }
}
