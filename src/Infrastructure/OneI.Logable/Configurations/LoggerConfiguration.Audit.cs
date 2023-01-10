namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;

public partial class LoggerConfiguration
{
    private class LoggerAuditConfiguration : ILoggerAuditConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerAuditConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration Attach(AuditDelegate audit)
        {
            var middleware = new AuditMiddleware(c => audit(c));

            _parent._lastComponents.Add(next => context => middleware.Invoke(context, next));

            return _parent;
        }

        public ILoggerConfiguration Attach(Action<LoggerContext> audit)
        {
            var middleware = new AuditMiddleware(audit);

            _parent._lastComponents.Add(next => context => middleware.Invoke(context, next));

            return _parent;
        }

        public ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, AuditDelegate audit)
        {
            var middleware = new ConditionalMiddleware(condition, new AuditMiddleware(c => audit(c)));

            _parent._lastComponents.Add(next => context => middleware.Invoke(context, next));

            return _parent;
        }

        public ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> audit)
        {
            var middleware = new ConditionalMiddleware(condition, new AuditMiddleware(audit));

            _parent._lastComponents.Add(next => context => middleware.Invoke(context, next));

            return _parent;
        }
    }
}
