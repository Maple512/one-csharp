namespace OneI.Hostable;

public interface IHostLifetime
{
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
