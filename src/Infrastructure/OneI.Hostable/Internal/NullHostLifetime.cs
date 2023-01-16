namespace OneI.Hostable.Internal;

using System.Threading;
using System.Threading.Tasks;

public class NullHostLifetime : IHostLifetime
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
