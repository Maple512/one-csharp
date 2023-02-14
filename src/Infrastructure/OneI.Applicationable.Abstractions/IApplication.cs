namespace OneI.Applicationable;

public interface IApplication
{
    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
