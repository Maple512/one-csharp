namespace OneI.Openable.Connections.Sockets;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OneI.Logable;

public sealed class SocketTransportFactory : IConnectionListenerFactory, IConnectionListenerFactorySelector
{
    private SocketTransportOptions _options;
    private ILogger _logger;

    public SocketTransportFactory(
        IOptions<SocketTransportOptions> options,
        ILogger logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public bool CanBind(EndPoint endpoint) => endpoint is IPEndPoint or UnixDomainSocketEndPoint or FileHandleEndPoint;

    public ValueTask<IConnectionListener> CreateAsync(EndPoint point, CancellationToken cancellationToken = default)
    {
        var transport = new SocketConnectionListener(point, _options, _logger);

        transport.Bind();

        return new(transport);
    }
}
