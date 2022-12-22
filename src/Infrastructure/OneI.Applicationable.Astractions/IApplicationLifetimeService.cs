namespace OneI.Applicationable;

using System.Threading;
/// <summary>
/// The application lifetime service.
/// </summary>

public interface IApplicationLifetimeService
{
    /// <summary>
    /// Gets the started.
    /// </summary>
    CancellationToken Started { get; }

    /// <summary>
    /// Gets the stopping.
    /// </summary>
    CancellationToken Stopping { get; }

    /// <summary>
    /// Gets the stopped.
    /// </summary>
    CancellationToken Stopped { get; }

    /// <summary>
    /// Ons the application started.
    /// </summary>
    void OnApplicationStarted();

    /// <summary>
    /// Ons the application stopping.
    /// </summary>
    void OnApplicationStopping();

    /// <summary>
    /// Ons the application stopped.
    /// </summary>
    void OnApplicationStopped();
}
