namespace OneI.Applicationable;

public interface IApplicationLifetime
{
    CancellationToken Started { get; }

    CancellationToken Stopping { get; }

    CancellationToken Stopped { get; }

    void Stop();
}
