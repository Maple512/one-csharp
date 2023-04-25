namespace OneI.Openable.Connections;

using System.Net;

public interface IMultiplexedConnectionListenerFactory
{
    ValueTask<IMultiplexedConnectionListener> BindAsync(EndPoint endpoint, TlsConnectionCallbackOptions options, CancellationToken cancellationToken = default);
}
