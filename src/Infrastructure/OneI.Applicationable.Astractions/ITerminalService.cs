namespace OneI.Applicationable;

using System.Threading;
using System.Threading.Tasks;

public interface ITerminalService
{
    ValueTask WaitForStartAsync(CancellationToken cancellationToken);

    ValueTask StopAsync(CancellationToken cancellationToken);
}
