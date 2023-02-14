namespace OneI.Applicationable;

public interface IApplicationLifetime
{
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
