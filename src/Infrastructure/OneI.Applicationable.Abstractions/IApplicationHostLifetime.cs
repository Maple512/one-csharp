namespace OneI.Applicationable;

public interface IApplicationHostLifetime
{
    Task StartAsync(CancellationToken token);

    Task StopAsync(CancellationToken token);
}
