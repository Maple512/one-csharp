namespace OneI.Applicationable;

public interface IApplicationHost
{
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
