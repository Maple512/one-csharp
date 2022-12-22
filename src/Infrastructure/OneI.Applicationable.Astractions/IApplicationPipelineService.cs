namespace OneI.Applicationable;

using System.Threading;
using System.Threading.Tasks;
/// <summary>
/// The application pipeline service.
/// </summary>

public interface IApplicationPipelineService
{
    /// <summary>
    /// Starts the async.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the async.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}
