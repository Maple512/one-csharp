namespace OneI.Applicationable;

using System;
using System.Threading;
using Microsoft.Extensions.Logging;

public class ApplicationLifetimeService : IApplicationLifetimeService
{
    private readonly ILogger _logger;

    private readonly CancellationTokenSource _startedSource = new();
    private readonly CancellationTokenSource _stoppingSource = new();
    private readonly CancellationTokenSource _stoppedSource = new();

    public ApplicationLifetimeService(ILogger<ApplicationLifetimeService> logger) => _logger = logger;

    public CancellationToken Started => _startedSource.Token;

    public CancellationToken Stopping => _stoppingSource.Token;

    public CancellationToken Stopped => _stoppedSource.Token;

    public void OnApplicationStarted()
    {
        try
        {
            ExecuteLifetimeHandler(_startedSource);
        }
        catch(Exception ex)
        {
            _logger.ApplicationError(
                ApplicationDefinition.LoggerEvents.ApplicationStartupException,
                "An error occurred starting the application",
                ex);
        }
    }

    public void OnApplicationStopping()
    {
        try
        {
            ExecuteLifetimeHandler(_stoppingSource);
        }
        catch(Exception ex)
        {
            _logger.ApplicationError(
                ApplicationDefinition.LoggerEvents.ApplicationStartupException,
                "An error occurred stopping the application",
                ex);
        }
    }

    public void OnApplicationStopped()
    {
        try
        {
            ExecuteLifetimeHandler(_stoppedSource);
        }
        catch(Exception ex)
        {
            _logger.ApplicationError(
                ApplicationDefinition.LoggerEvents.ApplicationStartupException,
                "An error occurred stopping the application",
                ex);
        }
    }

    private static void ExecuteLifetimeHandler(CancellationTokenSource cancellationTokenSource)
    {
        if(cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        cancellationTokenSource.Cancel(false);
    }
}
