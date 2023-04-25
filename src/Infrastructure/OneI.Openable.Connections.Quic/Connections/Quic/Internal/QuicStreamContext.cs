namespace OneI.Openable.Connections.Quic.Internal;

using System.Buffers;
using System.IO.Pipelines;
using System.Net.Quic;
using OneI.Logable;

internal sealed class QuicStreamContext : TransportConnection, IPooledStream, IDisposable
{
    private const int AllocBufferMinSize = 4 * 1024;// 4KiB

    private static readonly ConnectionAbortedException SendGracefullyCompletedException = new("The QUIC transport's send loop completed gracefully.");

    internal Task _processing = Task.CompletedTask;

    private QuicStream? _stream;
    private QuicConnectionContext _connectionContext;
    private QuicTransportContext _transportContext;
    private Pipe _inputPipe;
    private Pipe _outputPipe;
    private IDuplexPipe _originalTransport;
    private IDuplexPipe _originalApplication;
    private CompletionPipeReader _transportPipeReader;
    private CompletionPipeWriter _transportPipeWriter;
    private ILogger _logger;
    private CancellationTokenSource? _streamClosedTokenSource;
    private string? _connectionId;
    private volatile Exception? _shutdownReadReason;
    private volatile Exception? _shutdownWriteReason;
    private volatile Exception? _shutdownReason;
    private volatile Exception? _writeAbortException;
    private bool _streamClosed;
    private bool _serverAborted;
    private bool _clientAbort;
    private readonly object _shutdownLock = new();
    private IDictionary<object, object?>? _persistentState;
    private long? _error;
    private List<OnCloseRegistration>? _onClosedRegistrations;

    public QuicStreamContext(QuicConnectionContext connectionContext, QuicTransportContext transportContext)
    {
        _connectionContext = connectionContext;
        _transportContext = transportContext;
        _logger = transportContext.Logger;
        MemoryPool = connectionContext.MemoryPool;
        RemoteEndPoint = connectionContext.RemoteEndPoint;
        LocalEndPoint = connectionContext.LocalEndPoint;

        var readBufferMaxSize = transportContext.Options.ReadBufferMaxLength ?? 0;
        var writeBufferMaxSize = transportContext.Options.WriteBufferMaxLength ?? 0;

        var inputOptions = new PipeOptions(MemoryPool, PipeScheduler.ThreadPool, PipeScheduler.Inline, readBufferMaxSize, readBufferMaxSize / 2, useSynchronizationContext: false);
        var outputOptions = new PipeOptions(MemoryPool, PipeScheduler.Inline, PipeScheduler.Inline, writeBufferMaxSize, writeBufferMaxSize / 2, useSynchronizationContext: false);

        _inputPipe = new(inputOptions);
        _outputPipe = new(outputOptions);

        _transportPipeReader = new(_inputPipe.Reader);
        _transportPipeWriter = new(_outputPipe.Writer);

        _originalApplication = new DuplexPipe(_outputPipe.Reader, _inputPipe.Writer);
        _originalTransport = new DuplexPipe(_transportPipeReader, _transportPipeWriter);
    }

    public override MemoryPool<byte> MemoryPool { get; }

    private PipeWriter Input => Application.Output;

    private PipeReader Output => Application.Input;

    public bool CanReuse { get; private set; }

    public long PoolExpirationTicks { get; set; }

    public override long Error
    {
        get => _error ?? -1;
        set => _error = value;
    }

    public void Initialize(QuicStream stream)
    {
        _stream = stream;

        _streamClosedTokenSource = null;
        _onClosedRegistrations?.Clear();

        CanRead = _stream.CanRead;
        CanWrite = _stream.CanWrite;
        _error = null;
        StreamId = _stream.Id;
        PoolExpirationTicks = 0;

        Transport = _originalTransport;
        Application = _originalApplication;

        _transportPipeReader.Reset();
        _transportPipeWriter.Reset();

        _connectionId = null;
        _shutdownReason = null;
        _writeAbortException = null;
        _streamClosed = false;
        _serverAborted = false;
        _clientAbort = false;

        if(CanReuse)
        {
            _inputPipe.Reset();
            _outputPipe.Reset();
        }

        CanReuse = false;
    }

    public override CancellationToken ClosedToken
    {
        get
        {
            _streamClosedTokenSource ??= new();

            return _streamClosedTokenSource.Token;
        }
        set => throw new NotSupportedException();
    }

    public override string ConnectionId
    {
        get => _connectionId ??= StringUtilities.ConcatAsHexSuffix(_connectionContext.ConnectionId, ':', (uint)StreamId);
        set => _connectionId = value;
    }

    public void Start()
    {
        _processing = StartAsync();
    }

    public void Dispose()
    {
        if(!_connectionContext.TryReturnStream(this))
        {
            // 遇到以下情况时释放
            // - 流不是双向的
            // - stream没有完成
            // - 池已经满了
            DisposeCore();
        }
    }

    // 当stream不在被重用时调用
    public void DisposeCore()
    {
        _streamClosedTokenSource?.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        if(_stream is null)
        {
            return;
        }

        _originalTransport.Input.Complete();
        _originalTransport.Output.Complete();

        await _processing;

        await _stream.DisposeAsync();

        lock(_shutdownLock)
        {
            // 排放流时不得计算CanReuse。当任何方法仍在运行时，都有可能接收到中止。
            // 在处理完成后计算CanReuse是安全的。
            // 对可以合并的内容要保守。
            // 仅池管道已成功完成且尚未中止的双向流。
            CanReuse = CanRead && CanWrite
                && _transportPipeReader.IsCompletedSuccessfully
                && _transportPipeWriter.IsCompletedSuccessfully
                && !_clientAbort
                && !_serverAborted
                && _shutdownReadReason is null
                && _shutdownWriteReason is null;

            if(!CanReuse)
            {
                DisposeCore();
            }

            _stream = null;
        }
    }

    public override void Abort(ConnectionAbortedException reason)
    {
        var stream = _stream;

        lock(_shutdownLock)
        {
            if(stream is null)
            {
                return;
            }

            if(_serverAborted)
            {
                return;
            }

            _serverAborted = true;
            _shutdownReason = reason;
        }

        var resolvedErrorCode = _error ?? 0;

        _logger.StreamAbort(ConnectionId, resolvedErrorCode, reason);

        if(stream.CanRead)
        {
            stream.Abort(QuicAbortDirection.Read, resolvedErrorCode);
        }

        if(stream.CanWrite)
        {
            stream.Abort(QuicAbortDirection.Write, resolvedErrorCode);
        }

        Output.CancelPendingRead();
    }

    private async Task StartAsync()
    {
        try
        {
            var receiveTask = ValueTask.CompletedTask;
            var sendTask = ValueTask.CompletedTask;

            if(_stream.CanRead)
            {
                receiveTask = ReceiveAsync();
            }

            if(_stream.CanWrite)
            {
                sendTask = SendAsync();
            }

            await receiveTask;

            await sendTask;

            CloseStream();
        }
        catch(Exception ex)
        {
            _logger.Error(ex, $"Unexpected exception in {nameof(QuicStreamContext)}.{nameof(StartAsync)}.");
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask ReceiveAsync()
    {
        Exception? error = null;

        try
        {
            var input = Input;
            while(true)
            {
                var buffer = input.GetMemory(AllocBufferMinSize);
                var receivedBytes = await _stream!.ReadAsync(buffer);
                if(receivedBytes is 0)
                {
                    break;
                }

                input.Advance(receivedBytes);

                ValueTask<FlushResult> flushTask;

                if(_stream!.ReadsClosed.IsCompletedSuccessfully)
                {
                    var completeTask = input.CompleteAsync();
                    if(completeTask.IsCompletedSuccessfully)
                    {
                        completeTask.GetAwaiter().GetResult();

                        flushTask = ValueTask.FromResult(new FlushResult(false, true));
                    }
                    else
                    {
                        flushTask = AwaitCompleteTaskAsync(completeTask);
                    }
                }
                else
                {
                    flushTask = input.FlushAsync();
                }

                if(!flushTask.IsCompleted)
                {
                    // 暂停
                    _logger.StreamPause(ConnectionId);
                }

                var result = await flushTask;

                if(!flushTask.IsCompleted)
                {
                    // 重新开始
                    _logger.StreamResume(ConnectionId);
                }

                if(result.IsCanceled || result.IsCanceled)
                {
                    // 管道消费者已停机
                    break;
                }
            }
        }
        catch(QuicException ex) when(ex.QuicError is QuicError.StreamAborted or QuicError.ConnectionAborted)
        {
            _error = ex.ApplicationErrorCode;

            _logger.StreamAbortedRead(ConnectionId, ex.ApplicationErrorCode.GetValueOrDefault());

            error = new ConnectionResetException(ex.Message, ex);

            _clientAbort = true;
        }
        catch(QuicException ex) when(ex.QuicError is QuicError.ConnectionIdle)
        {
            _logger.StreamTimeoutRead(ConnectionId);

            error = new ConnectionResetException(ex.Message, ex);
        }
        catch(Exception ex)
        {
            error = ex;

            _logger.StreamError(ConnectionId, ex);
        }
        finally
        {
            Input.Complete(ResolveCompleteReceiveException(error));
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask SendAsync()
    {
        Exception? shutdownReason = null, unexpectedError = null;

        var waitForWritesClosedTask = WaitForWritesClosedAsync();

        try
        {
            var output = Output;
            while(true)
            {
                var result = await output.ReadAsync();
                if(result.IsCanceled)
                {
                    _writeAbortException?.ReThrow();

                    break;
                }

                var buffer = result.Buffer;

                var end = buffer.End;
                var isCompleted = result.IsCompleted;
                if(!buffer.IsEmpty)
                {
                    // single segment
                    if(buffer.IsSingleSegment)
                    {
                        await _stream!.WriteAsync(buffer.First, isCompleted);
                    }
                    else
                    {
                        var enumerator = buffer.GetEnumerator();
                        var isLastSegment = !enumerator.MoveNext();
                        while(!isLastSegment)
                        {
                            var currentSegment = enumerator.Current;

                            isLastSegment = !enumerator.MoveNext();

                            await _stream!.WriteAsync(currentSegment, isLastSegment && isCompleted);
                        }
                    }
                }

                output.AdvanceTo(end);

                if(isCompleted)
                {
                    // stream管道关闭
                    break;
                }
            }
        }
        catch(QuicException ex) when(ex.QuicError is QuicError.StreamAborted or QuicError.ConnectionAborted)
        {
            _error = ex.ApplicationErrorCode;

            _logger.StreamAbortedWrite(ConnectionId, _error ?? 0);

            shutdownReason = new ConnectionResetException(ex.Message, ex);

            _clientAbort = true;
        }
        catch(QuicException ex) when(ex.QuicError is QuicError.ConnectionIdle)
        {
            _logger.StreamTimeoutWrite(ConnectionId);

            shutdownReason = new ConnectionResetException(ex.Message, ex);
        }
        catch(QuicException ex) when(ex.QuicError is QuicError.OperationAborted)
        {
            shutdownReason = new ConnectionResetException(ex.Message, ex);
        }
        catch(Exception ex)
        {
            shutdownReason = unexpectedError = ex;

            _logger.StreamError(ConnectionId, ex);
        }
        finally
        {
            ShutdownWrite(shutdownReason);

            await waitForWritesClosedTask;

            // 
            Output.Complete(unexpectedError);

            // 取消挂起的刷新，以便取消暂停输入循环
            Input.CancelPendingFlush();
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask WaitForWritesClosedAsync()
    {
        Debug.Assert(_stream is not null);

        try
        {
            await _stream!.WritesClosed;
        }
        catch(Exception ex)
        {
            _writeAbortException = ex;
        }
        finally
        {
            Output.CancelPendingRead();
        }
    }

    private void ShutdownWrite(Exception? shutdownReason)
    {
        Debug.Assert(_stream is not null);

        try
        {
            lock(_shutdownLock)
            {
                _shutdownReason = _shutdownWriteReason ?? _shutdownReason ?? shutdownReason ?? SendGracefullyCompletedException;

                _logger.StreamShutdownWrite(ConnectionId, _shutdownReason);

                // 当完成写入时，才能正常关机
                if(_shutdownReason == SendGracefullyCompletedException)
                {
                    _stream.CompleteWrites();
                }
            }
        }
        catch(Exception ex)
        {
            // 忽略Shutdown时的所有错误，因为会删除stream
            _logger.Warning(ex, "Stream failed to gracefully shutdown.");
        }
    }

    private Exception? ResolveCompleteReceiveException(Exception? error)
    {
        return _shutdownReadReason ?? _shutdownReason ?? error;
    }

    private void CloseStream()
    {
        lock(_shutdownLock)
        {
            if(_streamClosed)
            {
                return;
            }

            _streamClosed = true;
        }

        var onClosed = _onClosedRegistrations;

        if(onClosed is not null)
        {
            for(var i = 0; i < onClosed.Count; i++)
            {
                var item = onClosed[i];
                item.Callback.Invoke(item.State);
            }
        }

        if(_streamClosedTokenSource is not null)
        {
            CancelConnectionClosedToken();
        }
    }

    private void CancelConnectionClosedToken()
    {
        Debug.Assert(_streamClosedTokenSource != null);

        try
        {
            _streamClosedTokenSource!.Cancel();
        }
        catch(Exception ex)
        {
            _logger.Error(ex, $"Unexpected exception in {nameof(QuicStreamContext)}.{nameof(CancelConnectionClosedToken)}.");
        }
    }

    private static async ValueTask<FlushResult> AwaitCompleteTaskAsync(ValueTask task)
    {
        await task;

        return new FlushResult(false, true);
    }

    private readonly record struct OnCloseRegistration(Action<object?> Callback, object? State);
}
