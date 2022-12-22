namespace OneI.Applicationable;

using System.Threading;
using System.Threading.Tasks;
/// <summary>
/// The terminal service.
/// </summary>

public interface ITerminalService
{
    /// <summary>
    /// Waits the for start async.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask WaitForStartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops the async.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask StopAsync(CancellationToken cancellationToken);
}
