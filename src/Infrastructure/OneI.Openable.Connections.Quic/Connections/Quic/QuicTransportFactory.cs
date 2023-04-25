namespace OneI.Openable.Connections.Quic;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OneI.Logable;
using OneI.Openable.Connections.Quic.Internal;

public sealed class QuicTransportFactory : IMultiplexedConnectionListenerFactory, IConnectionListenerFactorySelector
{
    private ILogger _logger;
    private QuicTransportOptions _options;

    public QuicTransportFactory(
        IOptions<QuicTransportOptions> options,
        ILogger logger)
    {
        _logger = logger.ForContext("OneI.Openable.Connections.Quic");
        _options = options.Value;
    }

    public async ValueTask<IMultiplexedConnectionListener> BindAsync(EndPoint endpoint, TlsConnectionCallbackOptions options, CancellationToken cancellationToken = default)
    {
        if(options is null)
        {
            throw new InvalidOperationException("Couldn't find HTTPS configuration for QUIC transport.");
        }

        if(options.Protocols is not { Count: > 0 })
        {
            throw new InvalidOperationException("No application protocols specified for QUIC transport.");
        }

        var transport = new QuicConnectionListener(_options, _logger, endpoint, options);

        await transport.CreateListenerAsync();

        return transport;
    }

    public bool CanBind(EndPoint endpoint)
    {
        throw new NotImplementedException();
    }
}
