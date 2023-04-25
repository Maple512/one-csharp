namespace OneI.Openable.Connections.Quic.Internal;

using System.Net.Quic;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using OneI.Diagnostics;
using OneI.Logable;
using OneI.Openable.Connections;
using OneI.Openable.Connections.Features;

internal sealed class QuicConnectionContext : TransportMultiplexedConnection
{
    internal PooledStreamStack<QuicStreamContext> StreamPool;
    private bool _streamPoolHeartbeatInitialized;
    private long? _error;
    private X509Certificate2? _clientCert;
    private Task<X509Certificate2?>? _clientCertTask;
    private long _heartbeatTicks;
    private readonly object _poolLock = new();
    private readonly object _shutdownLock = new();
    private QuicConnection _connection;
    private QuicTransportContext _context;
    private ILogger _logger;
    private CancellationTokenSource _closedTokenSource = new();
    private IConnectionHeartbeatFeature? _connectionHeartbeatFeature;
    private IStreamDirectionFeature? _streamDirectionFeature;
    private Task? _closeTask;
    private ExceptionDispatchInfo? _abortReason;

    internal const int InitialStreamPoolSize = 5;
    internal const int StreamPoolMaxSize = 100;
    internal const long StreamPoolExpiryTicks = TimeSpan.TicksPerSecond * 5;

    public QuicConnectionContext(
        QuicConnection connection,
        QuicTransportContext context,
        IConnectionHeartbeatFeature? connectionHeartbeatFeature = null,
        IStreamDirectionFeature? streamDirectionFeature = null)
    {
        _logger = context.Logger;
        _context = context;
        _connection = connection;

        ClosedToken = _closedTokenSource.Token;

        StreamPool = new PooledStreamStack<QuicStreamContext>(InitialStreamPoolSize);

        RemoteEndPoint = connection.RemoteEndPoint;
        LocalEndPoint = connection.LocalEndPoint;

        _connectionHeartbeatFeature = connectionHeartbeatFeature;
        _streamDirectionFeature = streamDirectionFeature;
    }

    public long Error
    {
        get => _error ?? -1;
        set => _error = value;
    }

    public QuicConnection QuicConnection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _connection;
    }

    public X509Certificate2? ClientCertificate
    {
        get => _clientCert ??= ConvertToX509Certificate2(_connection.RemoteCertificate);
        set
        {
            _clientCert = value;
            _clientCertTask = Task.FromResult(value);
        }
    }

    public Task<X509Certificate2?> GetClientCertificateAsync(CancellationToken cancellationToken)
    {
        return _clientCertTask ??= Task.FromResult(ClientCertificate);
    }

    public override void Abort()
    {
        Abort(new("The connection was aborted by the application via MultiplexedConnectionContext.Abort()."));
    }

    public override void Abort(ConnectionAbortedException abortReason)
    {
        lock(_shutdownLock)
        {
            // Check if connection has already been already aborted.
            if(_abortReason != null || _closeTask != null)
            {
                return;
            }

            var resolvedErrorCode = _error ?? 0;

            _abortReason = ExceptionDispatchInfo.Capture(abortReason);

            _logger.ConnectionAbort(ConnectionId, resolvedErrorCode, abortReason);

            _closeTask = _connection.CloseAsync(errorCode: resolvedErrorCode).AsTask();
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public override async ValueTask<ConnectionContext?> AcceptAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var stream = await _connection.AcceptInboundStreamAsync(cancellationToken);

            QuicStreamContext? context = null;

            if(stream.CanRead && stream.CanWrite)
            {
                lock(_poolLock)
                {
                    _ = StreamPool.TryPop(out context);
                }
            }

            if(context is null)
            {
                context = new(this, _context);
                context.Initialize(stream);
            }
            else
            {
                context.Initialize(stream);
                _logger.StreamReused(ConnectionId);
            }

            context.Start();

            _logger.AcceptedStream(context);

            return context;
        }
        catch(QuicException ex) when(ex.QuicError is QuicError.ConnectionAborted)
        {
            _error = ex.ApplicationErrorCode;

            _logger.ConnectionAborted(ConnectionId, _error ?? 0, ex);

            _ = ThreadPool.UnsafeQueueUserWorkItem(state => state.CancelConnectionClosedToken(), this, false);

            throw new ConnectionResetException(ex.Message, ex);
        }
        catch(QuicException ex) when(ex.QuicError is QuicError.OperationAborted)
        {
            lock(_shutdownLock)
            {
                if(_abortReason is null)
                {
                    Abort(new("Unexpected exception when accepting stream", ex));
                }

                _abortReason!.Throw();
            }
        }
        catch(QuicException ex) when(ex.QuicError is QuicError.ConnectionTimeout)
        {
            lock(_shutdownLock)
            {
                if(_abortReason is null)
                {
                    Abort(new("The connection timed out waiting for a response from the peer.", ex));
                }

                _abortReason!.Throw();
            }
        }
        catch(OperationCanceledException)
        {
            lock(_shutdownLock)
            {
                _abortReason?.Throw();
            }
        }
        catch(Exception ex)
        {
            Debug.Fail($"Unexpected exception in {nameof(QuicConnectionContext)}.{nameof(AcceptAsync)}: {ex}");

            throw;
        }

        return null;
    }

    public override async ValueTask<ConnectionContext?> ConnectAsync(CancellationToken cancellationToken = default)
    {
        QuicStream quicStream;

        if(_streamDirectionFeature is not null)
        {
            if(_streamDirectionFeature.CanRead)
            {
                quicStream = await _connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional, cancellationToken);
            }
            else
            {
                quicStream = await _connection.OpenOutboundStreamAsync(QuicStreamType.Unidirectional, cancellationToken);
            }
        }
        else
        {
            quicStream = await _connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional, cancellationToken);
        }

        var context = new QuicStreamContext(this, _context);
        context.Initialize(quicStream);
        context.Start();

        _logger.ConnectedStream(context);

        return context;
    }

    internal bool TryReturnStream(QuicStreamContext stream)
    {
        lock(_poolLock)
        {
            if(!_streamPoolHeartbeatInitialized)
            {
                // Kestrel将心跳功能添加到连接功能中。
                // 在添加的功能和提供服务的连接之间，上下文中没有任何事件，
                // 因此在第一次将流添加到连接的流池时初始化心跳。
                if(_connectionHeartbeatFeature is null)
                {
                    throw new InvalidOperationException($"Required {nameof(IConnectionHeartbeatFeature)} not found in {nameof(QuicConnectionContext)}");
                }

                _connectionHeartbeatFeature.OnHeartbeat(static state => state.RemoveExpiredStreams(), this);

                var now = Clock.Now().Ticks;

                Volatile.Write(ref _heartbeatTicks, now);

                _streamPoolHeartbeatInitialized = true;
            }

            if(stream.CanReuse && StreamPool.Count < StreamPoolMaxSize)
            {
                stream.PoolExpirationTicks = Volatile.Read(ref _heartbeatTicks) + StreamPoolExpiryTicks;

                StreamPool.Push(stream);

                _logger.StreamPooledReuse(ConnectionId);

                return true;
            }
        }

        return false;
    }

    private void RemoveExpiredStreams()
    {
        lock(_poolLock)
        {
            var now = Clock.Now().Ticks;

            Volatile.Write(ref _heartbeatTicks, now);

            StreamPool.RemoveExpired(now);
        }
    }

    private void CancelConnectionClosedToken()
    {
        try
        {
            _closedTokenSource.Cancel();
        }
        catch(Exception ex)
        {
            _logger.Error(ex, $"Unexpected exception in {nameof(QuicConnectionContext)}.{nameof(CancelConnectionClosedToken)}");
        }
    }

    private static X509Certificate2? ConvertToX509Certificate2(X509Certificate? certificate)
    {
        if(certificate == null)
        {
            return null;
        }

        if(certificate is X509Certificate2 cert)
        {
            return cert;
        }

        return new X509Certificate2(certificate);
    }
}
