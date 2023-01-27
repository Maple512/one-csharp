namespace OneI.Logable;

using Configurations;

public partial class LoggerConfiguration
{
    private class AuditConfiguration : ILoggerAuditConfiguration
    {
        private readonly ILoggerConfiguration _parent;

        public AuditConfiguration(ILoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration Attach(ILoggerSink auditor)
        {
            _parent.Sink.Use(auditor);

            return _parent;
        }
    }
}
