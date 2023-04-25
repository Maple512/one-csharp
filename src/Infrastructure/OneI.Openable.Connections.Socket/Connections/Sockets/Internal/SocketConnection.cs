namespace OneI.Openable.Connections.Sockets.Internal;

using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using OneI.Logable;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal sealed class SocketConnection : TransportConnection
{
    private static readonly int AllocBufferMinLength = PinnedBlockMemoryPool.BlockSize / 2;

    private readonly Socket _socket;
    private readonly ILogger _logger;
    private readonly SocketReceiver _receiver;
    private SocketSender? _sender;
    private readonly SocketSenderPool _senderPool;
    private readonly IDuplexPipe _originalTransport;
    private readonly CancellationTokenSource _closedTokenSource = new();

    private readonly object _shutdownLock = new();

    private volatile bool _socketDisposed;
    private volatile Exception? _shutdownReason;
    private Task? _sendingTask;
    private Task? _receivingTask;
    private readonly TaskCompletionSource _waitForConnectionClosedTcs = new();
    private bool _connectionClosed;
    private readonly bool _waitForData;

    internal SocketConnection(
        Socket socket,
        MemoryPool<byte> memoryPool,
        PipeScheduler pipeScheduler,
        ILogger logger,
        SocketSenderPool senderPool,
        PipeOptions inputOptions,
        PipeOptions outputOptions,
        bool waitForData = true)
    {
        _socket = socket;
        MemoryPool = memoryPool;
        _logger = logger;
        _waitForData = waitForData;
        _senderPool = senderPool;

        LocalEndPoint = socket.LocalEndPoint;
        RemoteEndPoint = socket.RemoteEndPoint;

        ClosedToken = _closedTokenSource.Token;

        _receiver = new SocketReceiver(pipeScheduler);

        var pair = DuplexPipe.CreateConnection(inputOptions, outputOptions);

        Transport = _originalTransport = pair.ClientPipe;
        Application = pair.ServerPipe;
    }

    public PipeWriter Input => Application.Output;

    public PipeReader Output => Transport.Input;

    public override MemoryPool<byte> MemoryPool { get; }

    public override Socket Socket => _socket;

    public void Start()
    {
        try
        {
            _receivingTask = Receive();
            _sendingTask = Send();
        }
        catch(Exception ex)
        {
            _logger.Error(ex, $"Unexpected exception in {nameof(SocketConnection)}.{nameof(Start)}");
        }
    }

    public override void Abort(ConnectionAbortedException reason)
    {
        // 尝试优雅地关闭套接字以匹配libuv行为。
        Shutdown(reason);

        // 取消处理在调用关机后发送循环，以确保设置正确的_shutdownReason
        Output.CancelPendingRead();
    }

    public override async ValueTask DisposeAsync()
    {
        _originalTransport.Input.Complete();
        _originalTransport.Output.Complete();

        try
        {
            if(_receivingTask != null)
            {
                await _receivingTask;
            }

            if(_sendingTask != null)
            {
                await _sendingTask;
            }
        }
        catch(Exception ex)
        {
            _logger.Error(ex, $"Unexpected exception in {nameof(SocketConnection)}.{nameof(Start)}");
        }
        finally
        {
            _receiver.Dispose();

            _sender?.Dispose();
        }

        _closedTokenSource.Dispose();
    }

    private async Task Receive()
    {
        Exception? error = null;

        try
        {
            while(true)
            {
                if(_waitForData)
                {
                    var waitForDataResult = await _receiver.WaitForDataAsync(_socket);

                    if(!IsNormalCompletion(waitForDataResult, error))
                    {
                        break;
                    }
                }

                var buffer = Input.GetMemory(AllocBufferMinLength);

                var receiveResult = await _receiver.ReceiveAsync(_socket, buffer);

                if(!IsNormalCompletion(receiveResult, error))
                {
                    break;
                }

                var receivedBytes = receiveResult.TransferredBytes;
                if(receivedBytes == 0)
                {
                    // TCP FIN状态：关闭链接
                    _logger.Debug($"Connection id \"{ConnectionId}\" received FIN.");
                    break;
                }

                Input.Advance(receivedBytes);

                var flushTask = Input.FlushAsync();

                var paused = !flushTask.IsCompleted;
                if(paused)
                {
                    _logger.ConnectionPaused(ConnectionId);
                }

                var flushedResult = await flushTask;

                if(flushedResult.IsCompleted || flushedResult.IsCanceled)
                {
                    // 通道消费者已停机
                    break;
                }
            }
        }
        catch(ObjectDisposedException ex)
        {
            error = ex;
            if(!_socketDisposed)
            {
                _logger.ConnectionError(ConnectionId, error);
            }
        }
        catch(Exception ex)
        {
            error = ex;
            _logger.ConnectionError(ConnectionId, error);
        }
        finally
        {
            Input.Complete(_shutdownReason ?? error);

            CloseConnection();

            await _waitForConnectionClosedTcs.Task;
        }
    }

    async Task Send()
    {
        Exception? shutdownError = null, unexpectedError = null;

        try
        {
            while(true)
            {
                var result = await Output.ReadAsync();

                if(result.IsCanceled)
                {
                    break;
                }

                var buffer = result.Buffer;
                if(!buffer.IsEmpty)
                {
                    _sender = _senderPool.Rent();

                    var transferResult = await _sender.SendAsync(_socket, buffer);

                    if(transferResult.HasError)
                    {
                        if(IsConnectionResetError(transferResult.Error.SocketErrorCode))
                        {
                            var ex = transferResult.Error;
                            shutdownError = new ConnectionResetException(ex.Message, ex);

                            _logger.ConnectionReset(ConnectionId);

                            break;
                        }

                        if(IsConnectionAbortError(transferResult.Error.SocketErrorCode))
                        {
                            shutdownError = transferResult.Error;
                            break;
                        }

                        unexpectedError = shutdownError = transferResult.Error;
                    }

                    // 如果出现异常，不会返回池，并且保留已分配的_sender，以便在StartAsync中处理它
                    _senderPool.Return(_sender);
                    _sender = null;
                }

                Output.AdvanceTo(buffer.End);

                if(result.IsCompleted)
                {
                    break;
                }
            }
        }
        catch(ObjectDisposedException ex)
        {
            shutdownError = ex;
        }
        catch(Exception ex)
        {
            unexpectedError = shutdownError = ex;

            _logger.ConnectionError(ConnectionId, ex);
        }
        finally
        {
            Shutdown(shutdownError);

            // 释放Socket后，结束输出
            Output.Complete(unexpectedError);

            // 取消任何挂起的刷新，以便取消暂停输入循环
            Input.CancelPendingFlush();
        }
    }

    private static bool IsConnectionResetError(SocketError error)
    {
        return error is SocketError.ConnectionReset
            or SocketError.Interrupted
            || (error is SocketError.InvalidArgument && !OperatingSystem.IsWindows());
    }

    static bool IsConnectionAbortError(SocketError error)
    {
        return error is SocketError.OperationAborted
            or SocketError.Interrupted
            || (error is SocketError.InvalidArgument && !OperatingSystem.IsWindows());
    }

    void Shutdown(Exception? error)
    {
        lock(_shutdownLock)
        {
            if(_socketDisposed) return;

            _socketDisposed = true;

            _shutdownReason = error ?? new ConnectionAbortedException("The Socket transport's send loop completed gracefully.");

            _logger.ConnectionWriteFIN(ConnectionId, _shutdownReason);

            try
            {
                // 停止发送和接收
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch { }

            _socket.Dispose();
        }
    }

    void CloseConnection()
    {
        if(_connectionClosed) return;

        _connectionClosed = true;

        ThreadPool.UnsafeQueueUserWorkItem(state =>
        {
            state.CancelConnectionClosedToken();

            state._waitForConnectionClosedTcs.TrySetResult();
        }, this, false);
    }

    void CancelConnectionClosedToken()
    {
        try
        {
            _closedTokenSource.Cancel();
        }
        catch(Exception ex)
        {
            _logger.Error(ex, $"Unexpected exception in {nameof(SocketConnection)}.{nameof(CancelConnectionClosedToken)}.");
        }
    }

    bool IsNormalCompletion(SocketOperationResult result, Exception? error)
    {
        // TODO: 测试error的值
        Debugger.Break();
        if(!result.HasError)
        {
            return true;
        }

        if(IsConnectionResetError(result.Error.SocketErrorCode))
        {
            var exception = result.Error;
            error = new ConnectionResetException(exception.Message, exception);

            if(!_socketDisposed)
            {
                _logger.ConnectionReset(ConnectionId);
            }

            return false;
        }

        if(IsConnectionAbortError(result.Error.SocketErrorCode))
        {
            error = result.Error;

            if(!_socketDisposed)
            {
                _logger.ConnectionError(ConnectionId, error);
            }
            return false;
        }

        error = result.Error;
        _logger.ConnectionError(ConnectionId, error);

        return false;
    }
}
