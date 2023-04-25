namespace OneI.Openable.Connections;

using System.Net;

public interface IConnectionListenerFactory
{
    ValueTask<IConnectionListener> CreateAsync(EndPoint point, CancellationToken cancellationToken = default);
}
