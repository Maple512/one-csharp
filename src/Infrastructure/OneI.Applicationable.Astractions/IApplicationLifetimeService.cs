namespace OneI.Applicationable;

using System.Threading;

public interface IApplicationLifetimeService
{
    CancellationToken Started { get; }

    CancellationToken Stopping { get; }

    CancellationToken Stopped { get; }

    void OnApplicationStarted();

    void OnApplicationStopping();

    void OnApplicationStopped();
}
