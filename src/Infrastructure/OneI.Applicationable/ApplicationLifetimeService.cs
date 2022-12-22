namespace OneI.Applicationable;

using System;
using System.Threading;
using Microsoft.Extensions.Logging;
/// <summary>
/// The application lifetime service.
/// </summary>

public class ApplicationLifetimeService : IApplicationLifetimeService
{
    private readonly ILogger _logger;

    private readonly CancellationTokenSource _startedSource = new();
    private readonly CancellationTokenSource _stoppingSource = new();
    private readonly CancellationTokenSource _stoppedSource = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLifetimeService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public ApplicationLifetimeService(ILogger<ApplicationLifetimeService> logger) => _logger = logger;

    /// <summary>
    /// Gets the started.
    /// </summary>
    public CancellationToken Started => _startedSource.Token;

    /// <summary>
    /// Gets the stopping.
    /// </summary>
    public CancellationToken Stopping => _stoppingSource.Token;

    /// <summary>
    /// Gets the stopped.
    /// </summary>
    public CancellationToken Stopped => _stoppedSource.Token;

    /// <summary>
    /// Ons the application started.
    /// </summary>
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

    /// <summary>
    /// Ons the application stopping.
    /// </summary>
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

    /// <summary>
    /// Ons the application stopped.
    /// </summary>
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

    /// <summary>
    /// Executes the lifetime handler.
    /// </summary>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    private static void ExecuteLifetimeHandler(CancellationTokenSource cancellationTokenSource)
    {
        if(cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        cancellationTokenSource.Cancel(false);
    }
}
