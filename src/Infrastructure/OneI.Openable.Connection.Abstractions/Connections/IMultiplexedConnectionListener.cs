namespace OneI.Openable.Connections;

using System.Net;

public interface IMultiplexedConnectionListener : IAsyncDisposable
{
    EndPoint EndPoint { get; }

    ValueTask UnbindAsync(CancellationToken cancellationToken = default);

    ValueTask<MultiplexedConnectionContext?> AcceptAsync(CancellationToken cancellationToken = default);
}
