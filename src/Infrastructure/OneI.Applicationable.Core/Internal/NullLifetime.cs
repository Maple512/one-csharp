namespace OneI.Applicationable.Internal;

using System.Threading;
using System.Threading.Tasks;

internal sealed class NullLifetime : IApplicationHostLifetime
{
    public Task StartAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
