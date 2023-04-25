namespace OneI.Openable.Connections.Quic.Internal;

using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using OneI.Logable;

internal sealed class QuicConnectionListener : IMultiplexedConnectionListener, IAsyncDisposable
{
    ILogger _logger;
    TlsConnectionCallbackOptions _tlsConnectionCallbackOptions;
    QuicTransportContext _context;
    QuicListenerOptions _listenerOptions;

    internal ConditionalWeakTable<QuicConnection, QuicConnectionContext> _pendingConnections;
    private bool _disposed;
    private QuicListener? _listener;

    public QuicConnectionListener(
        QuicTransportOptions options,
        ILogger logger,
        EndPoint endPoint,
        TlsConnectionCallbackOptions tlsConnectionCallbackOptions)
    {
        if(!QuicListener.IsSupported)
        {
            throw new NotSupportedException();
        }

        if(endPoint is not IPEndPoint listenEndPoint)
        {
            throw new InvalidOperationException($"QUIC doesn't support listening on the configured endpoint type. Expected {nameof(IPEndPoint)} bug got {endPoint.GetType().Name}.");
        }

        if(tlsConnectionCallbackOptions.Protocols.Count is 0)
        {
            throw new InvalidOperationException("No Application protocols specified.");
        }

        _pendingConnections = new();
        _logger = logger;
        _tlsConnectionCallbackOptions = tlsConnectionCallbackOptions;
        _context = new(_logger, options);
        _listenerOptions = new QuicListenerOptions
        {
            ApplicationProtocols = _tlsConnectionCallbackOptions.Protocols,
            ListenEndPoint = listenEndPoint,
            ListenBacklog = options.PendingConnectionQueueMaxLength,
            ConnectionOptionsCallback = async (connection, helloInfo, cancellationToken) =>
            {
                // 在回调中创建连接上下文，因为它已传递给连接回调。
                // 一旦AcceptConnectionAsync完成等待，就会读取该字段。
                var currentAcceptConnection = new QuicConnectionContext(connection, _context);

                _pendingConnections.Add(connection, currentAcceptConnection);

                var context = new TlsConnectionCallbackContext
                {
                    ClientHelloInfo = helloInfo,
                    State = _tlsConnectionCallbackOptions.OnConnectionState,
                    ConnectionContext = currentAcceptConnection,
                };

                var serverAuthenticationOptions = await _tlsConnectionCallbackOptions.OnConnection.Invoke(context, cancellationToken);

                serverAuthenticationOptions.ApplicationProtocols ??= _tlsConnectionCallbackOptions.Protocols;

                ValidateServerAuthenticationOptions(serverAuthenticationOptions);

                var connectionOptions = new QuicServerConnectionOptions
                {
                    ServerAuthenticationOptions = serverAuthenticationOptions,
                    IdleTimeout = Timeout.InfiniteTimeSpan,
                    MaxInboundBidirectionalStreams = options.BidirectionalStreamMaxCount,
                    MaxInboundUnidirectionalStreams = options.UnidirectionalStreamMaxCount,
                    DefaultCloseErrorCode = options.CloseErrorCodeDefault,
                    DefaultStreamErrorCode = options.StreamErrorCodeDefault,
                };

                return connectionOptions;
            },
        };

        EndPoint = listenEndPoint;
    }

    public EndPoint EndPoint { get; private set; }

    public async ValueTask CreateListenerAsync()
    {
        _logger.Debug($"QUIC listener starting with configured endpoint {_listenerOptions.ListenEndPoint}.");

        try
        {
            _listener = await QuicListener.ListenAsync(_listenerOptions);
        }
        catch(QuicException ex) when(ex.QuicError is QuicError.AddressInUse or QuicError.AlpnInUse)
        {
            // 根据使用场景中的地址，QUIC具有不同的错误状态
            // 当另一个进程正在使用UDP端口时，它的状态为AddressInUse
            // 当当前进程使用UDP端口和ALPN相同时，其状态为AlpnUse
            throw new AddressInUseException(ex.Message, ex);
        }

        // 可以使用0作为临时端口配置EndPoing，
        // 侦听器端点将临时端口（如127.0.0.1:0）解析为实际端口
        // 因此我们需要更新公共侦听端口
        EndPoint = _listener.LocalEndPoint;
    }

    public async ValueTask<MultiplexedConnectionContext?> AcceptAsync(CancellationToken cancellationToken = default)
    {
        if(_listener == null)
        {
            throw new InvalidOperationException($"The listener needs to be initialzied by calling {nameof(CreateListenerAsync)}.");
        }

        while(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var connection = await _listener.AcceptConnectionAsync(cancellationToken);

                if(!_pendingConnections.TryGetValue(connection, out QuicConnectionContext? context))
                {
                    throw new InvalidOperationException($"Couldn't find {nameof(ConnectionContext)} for {nameof(QuicConnection)}.");
                }
                else
                {
                    _pendingConnections.Remove(connection);
                }

                Debug.Assert(context is not null);
                Debug.Assert(context.QuicConnection == connection);

                _logger.AcceptedConnection(context.ConnectionId);

                return context;
            }
            catch(QuicException ex) when(ex.QuicError is QuicError.OperationAborted)
            {
                // 当正在进行 accept 并且 listener is unbind/disposed时，会报告OperationAborted
                _logger.Debug(ex, "QUIC listener aborted.");

                return null;
            }
            catch(ObjectDisposedException ex)
            {
                // 在listener is unbind/disposed 后启动accept时，会报告ObjectDisposedException
                _logger.Debug(ex, "QUIC listener aborted.");

                return null;
            }
            catch(Exception ex)
            {
                // 如果客户端因为证书无效而拒绝连接，那么AcceptConnectionAsync将抛出异常。
                // ConnectionOptionsCallback内部引发的错误也可以从AcceptConnectionAsync引发。
                // 这些都是可以恢复的错误，不需要停止接收连接
                _logger.Debug(ex, "QUIC listener connection failed.");
            }
        }

        return null;
    }

    public async ValueTask DisposeAsync()
    {
        if(_disposed)
        {
            return;
        }

        if(_listener is not null)
        {
            await _listener.DisposeAsync();
        }

        _disposed = true;
    }

    public ValueTask UnbindAsync(CancellationToken cancellationToken = default) 
        => DisposeAsync();

    void ValidateServerAuthenticationOptions(SslServerAuthenticationOptions options)
    {
        if(options.ServerCertificate is null
            && options.ServerCertificateContext is null
            && options.ServerCertificateSelectionCallback is null)
        {
            _logger.Warning($"{nameof(SslServerAuthenticationOptions)} must provide a server certificate using {nameof(SslServerAuthenticationOptions.ServerCertificate)}, {nameof(SslServerAuthenticationOptions.ServerCertificateContext)}, or {nameof(SslServerAuthenticationOptions.ServerCertificateSelectionCallback)}.");
        }

        if(options.ApplicationProtocols is null or { Count: 0 })
        {
            _logger.Warning($"{nameof(SslServerAuthenticationOptions)} must provide at least one application protocol using {nameof(SslServerAuthenticationOptions.ApplicationProtocols)}.");
        }
    }
}
