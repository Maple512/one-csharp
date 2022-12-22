namespace OneI.Applicationable;

using System;
using System.Threading;
using System.Threading.Tasks;
/// <summary>
/// The application.
/// </summary>

public interface IApplication
{
    /// <summary>
    /// Gets the service provider.
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Starts the async.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the async.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
