namespace OneI.Applicationable;

using Microsoft.Extensions.Configuration;
using OneI.Logable;

public interface IApplication
{
    IServiceProvider Services { get; }

    IConfiguration Configuration { get; }

    IApplicationEnvironment Environment { get; }

    ILogger Logger { get; }

    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
