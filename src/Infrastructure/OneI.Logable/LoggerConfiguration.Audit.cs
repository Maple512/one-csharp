namespace OneI.Logable;

using OneI.Logable.Configurations;

public partial class LoggerConfiguration
{
    private class AuditConfiguration : ILoggerAuditConfiguration
    {
        private readonly ILoggerConfiguration _parent;

        public AuditConfiguration(ILoggerConfiguration parent) => _parent = parent;

        public ILoggerConfiguration Attach(ILoggerSink auditor)
        {
            _ = _parent.Sink.Use(auditor);

            return _parent;
        }
    }
}
