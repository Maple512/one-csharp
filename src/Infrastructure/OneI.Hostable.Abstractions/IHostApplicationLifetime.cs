namespace OneI.Hostable;

public interface IHostApplicationLifetime
{
    CancellationToken Started { get; }

    CancellationToken Stopping { get; }

    CancellationToken Stopped { get; }

    void StopApplication();
}
