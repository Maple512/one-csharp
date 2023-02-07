namespace OneI.Logable.Extensions;

using System.Diagnostics;
using System.Runtime.Versioning;

[SupportedOSPlatform(SharedConstants.OSPlatform.Windows)]
public class EventSource_Test
{
    [Fact]
    public void add_event_source_log()
    {
        var logName = Randomizer.Latter(3);

        var logger = new LoggerConfiguration()
            .Sink.UseEventSource(configure =>
            {
                configure.LogName = logName;
                configure.SourceName = nameof(EventSource_Test);
                configure.DeleteExistedLogName = true;
            }).CreateLogger();

        logger.Information("EventSource Message");

        var logs = EventLog.GetEventLogs().FirstOrDefault(x => x.Log == logName);

        _ = logs.ShouldNotBeNull();
    }
}
