namespace OneI.Openable.Connections.Sockets;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using OneI.Logable;
using OneI.Openable.Connections.Sockets.Internal;

internal sealed class SocketConnectionListener : IConnectionListener
{
    private SocketConnectionContextFactory _factory;
    private ILogger _logger;
    private Socket? _socket;
    private SocketTransportOptions _options;

    public SocketConnectionListener(EndPoint endpoint,
        SocketTransportOptions options,
        ILogger logger)
    {
        EndPoint = endpoint;
        _options = options;
        _logger = logger.ForContext("OneI.Openable.Connections.Sockets");
        _factory = new(new(options), _logger);
    }

    public EndPoint EndPoint { get; private set; }

    public async ValueTask<ConnectionContext?> AcceptAsync(CancellationToken cancellationToken = default)
    {
        while(true)
        {
            try
            {
                var acceptSocket = await _socket!.AcceptAsync(cancellationToken);

                // 仅对基于TCP的端点不应用延迟
                if(acceptSocket.LocalEndPoint is IPEndPoint)
                {
                    acceptSocket.NoDelay = _options.NoDelay;
                }

                return _factory.Create(acceptSocket);
            }
            catch(ObjectDisposedException)
            {
                // 对UnbindAsync/DisposeAsync进行了调用，只返回null，表示我们已完成
                return null;
            }
            catch(SocketException ex) when(ex.SocketErrorCode is SocketError.OperationAborted)
            {
                // 对UnbindAsync/DisposeAsync进行了调用，只返回null，表示我们已完成
                return null;
            }
            catch(SocketException)
            {
                _logger.ConnectionReset("(null)");
            }
        }
    }

    public ValueTask UnbindAsync(CancellationToken cancellationToken = default)
    {
        _socket?.Dispose();

        return default;
    }

    public ValueTask DisposeAsync()
    {
        _socket?.Dispose();

        _factory.Dispose();

        return default;
    }

    internal void Bind()
    {
        if(_socket != null)
        {
            throw new InvalidOperationException("Connection is already bound.");
        }

        Socket listenSocket;
        try
        {
            listenSocket = _options.CreateBoundListenSocket(EndPoint);
        }
        catch(SocketException ex) when(ex.SocketErrorCode is SocketError.AddressAlreadyInUse)
        {
            throw new AddressInUseException(ex.Message, ex);
        }

        EndPoint = listenSocket.LocalEndPoint!;

        listenSocket.Listen(_options.WaitingConnectionQueueMaxLength);

        _socket = listenSocket;
    }
}
