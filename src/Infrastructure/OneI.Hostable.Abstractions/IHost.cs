namespace OneI.Hostable;

public interface IHost : IDisposable
{
    IServiceProvider Services { get; }

    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
