namespace OneI.Openable.Connections;

using System.Net;

public interface IMultiplexedConnectionFactory
{
    ValueTask<MultiplexedConnectionContext> ConnectionAsync(EndPoint endpoint, CancellationToken cancellationToken = default);
}
