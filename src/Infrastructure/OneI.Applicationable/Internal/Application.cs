namespace OneI.Applicationable.Internal;

using System.Threading;
using System.Threading.Tasks;

internal class Application : IApplication
{
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
