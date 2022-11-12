namespace OneI.Applicationable;

using System.Threading;
using System.Threading.Tasks;

public interface IApplicationPipelineService
{
    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
