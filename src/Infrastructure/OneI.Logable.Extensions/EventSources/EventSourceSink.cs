namespace OneI.Logable;

using System.Runtime.Versioning;
using Cysharp.Text;
using OneI.Logable.Templates;

[SupportedOSPlatform(SharedConstants.OSPlatform.Windows)]
internal class EventSourceSink : ILoggerSink
{
    private readonly EventLog _log;
    private readonly Func<LoggerMessageContext, bool>? _filter;

    public EventSourceSink(EventSourceOptions options)
    {
        _log = new EventLog(options.LogName, options.MachineName, options.SourceName);

        _filter = options.Filter;
    }

    public void Invoke(in LoggerContext context)
    {
        if(_filter?.Invoke(context.Message) != false)
        {
            using var writer = new ZStringWriter();

            context.WriteTo(writer);

            _log.WriteEntry(writer.ToString(), ToEventLogEntryType(context.Message.Level), context.EventId, 0);
        }
    }

    private static EventLogEntryType ToEventLogEntryType(LogLevel level) => level switch
    {
        LogLevel.Warning => EventLogEntryType.Warning,
        LogLevel.Fatal or LogLevel.Error => EventLogEntryType.Error,
        LogLevel.Verbose or LogLevel.Debug or LogLevel.Information or _ => EventLogEntryType.Information,
    };
}
