namespace OneI.Logable;

using System.Runtime.Versioning;

[SupportedOSPlatform(SharedConstants.OSPlatform.Windows)]
public class EventSourceOptions
{
    public const string EventId = nameof(EventId);

    public string? LogName { get; set; } = "Application";

    public string? SourceName { get; set; } = ".NET Runtime";

    public string? MachineName { get; set; } = ".";

    /// <summary>
    /// LogName与SourceName属于绑定关系，设置此属性为<see langword="true"/>，则删除已存在的绑定关系
    /// <para>如果存在但并未删除，则会触发异常</para>
    /// </summary>
    public bool DeleteExistedLogName { get; set; }

    public Func<LoggerMessageContext, bool>? Filter { get; set; }

    internal void VerifyAndCreateSource()
    {
        if(EventLog.SourceExists(SourceName, MachineName) == false)
        {
            EventLog.CreateEventSource(new EventSourceCreationData(SourceName, LogName)
            {
                MachineName = MachineName,
            });
        }

        var logName = EventLog.LogNameFromSourceName(SourceName, MachineName);
        if(logName is { Length: > 0 } && logName != LogName)
        {
            if(DeleteExistedLogName)
            {
                EventLog.Delete(logName, MachineName);
            }
            else
            {
                throw new ArgumentException($"The source '{SourceName}' is not registered in log '{LogName}'. (It is registered in log 'OneI'.) \" The Source and Log properties must be matched, or you may set Log to the empty string, and it will automatically be matched to the Source property.NoAccountInfo=Cannot obtain account information.");
            }
        }
    }
}
