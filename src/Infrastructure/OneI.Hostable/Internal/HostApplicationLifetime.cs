namespace OneI.Hostable.Internal;

using Microsoft.Extensions.Logging;
using static HostableConstants;

public class HostApplicationLifetime : IHostApplicationLifetime
{
    private readonly ILogger<IHostApplicationLifetime> _logger;
    private readonly CancellationTokenSource _started = new();
    private readonly CancellationTokenSource _stopping = new();
    private readonly CancellationTokenSource _stopped = new();

    public HostApplicationLifetime(ILogger<IHostApplicationLifetime> logger)
    {
        _logger = logger;
    }

    public CancellationToken Started => _started.Token;
    public CancellationToken Stopping => _stopping.Token;
    public CancellationToken Stopped => _stopped.Token;

    public void StopApplication()
    {
        lock(_stopping)
        {
            try
            {
                ExecuteHandlers(_stopping);
            }
            catch(Exception ex)
            {
                _logger.ApplicationError(LoggerEventIds.ApplicationStoppingException,
                                         "An error occurred stopping the application",
                                         ex);
            }
        }
    }

    public void NotifyStarted()
    {
        try
        {
            ExecuteHandlers(_started);
        }
        catch(Exception ex)
        {
            _logger.ApplicationError(LoggerEventIds.ApplicationStartupException,
                                     "An error occurred starting the application",
                                     ex);
        }
    }

    public void NotifyStopped()
    {
        try
        {
            ExecuteHandlers(_stopped);
        }
        catch(Exception ex)
        {
            _logger.ApplicationError(LoggerEventIds.ApplicationStoppedException,
                                     "An error occurred stopping the application",
                                     ex);
        }
    }

    private static void ExecuteHandlers(CancellationTokenSource handler)
    {
        if(handler.IsCancellationRequested)
        {
            return;
        }

        handler.Cancel(false);
    }
}
