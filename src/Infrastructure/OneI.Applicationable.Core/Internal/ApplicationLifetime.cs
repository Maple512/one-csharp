namespace OneI.Applicationable.Internal;

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OneI.Logable;

internal sealed class ApplicationLifetime : IApplicationLifetime
{
    CancellationTokenSource _started = new();
    CancellationTokenSource _stopping = new();
    CancellationTokenSource _stopped = new();

    ILogger _logger;

    public ApplicationLifetime(ILogger logger)
    {
        _logger = logger.ForContext("OneI.Applicationable.Lifetime");
    }

    public CancellationToken Started => _started.Token;

    public CancellationToken Stopping => _stopping.Token;

    public CancellationToken Stopped => _stopped.Token;

    public void Stop()
    {
        lock(_stopping)
        {
            try
            {
                Cancel(_stopping);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "An error occurred stopping the application.");
            }
        }
    }

    public void NotifyStarted()
    {
        try
        {
            Cancel(_started);
        }
        catch(Exception ex)
        {
            _logger.Error(ex, "An error occurred starting the application");
        }
    }

    public void NotifyStopped()
    {
        try
        {
            Cancel(_stopped);
        }
        catch(Exception ex)
        {
            _logger.Error(ex, "An error occurred starting the application");
        }
    }

    static void Cancel(CancellationTokenSource source)
    {
        if(source.IsCancellationRequested)
        {
            return;
        }

        source.Cancel(false);
    }
}
