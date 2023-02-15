namespace OneI.Applicationable;

public interface IApplicationStartingService
{
    Task StartAsync(CancellationToken token);

    Task StopAsync(CancellationToken token);
}
