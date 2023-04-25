namespace OneI.Openable.Connections.Sockets.Internal;

using System;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Tasks.Sources;

internal class SocketAwaitableEventArgs : SocketAsyncEventArgs, IValueTaskSource<SocketOperationResult>
{
    private static readonly Action<object?> _continuationCompleted = static _ => { };

    private readonly PipeScheduler _scheduler;
    private Action<object?>? _continuation;

    public SocketAwaitableEventArgs(PipeScheduler scheduler)
        : base(true)
    {
        _scheduler = scheduler;
    }

    public SocketOperationResult GetResult(short token)
    {
        _continuation = null;

        if(SocketError is not SocketError.Success)
        {
            return new SocketOperationResult(CreateException(SocketError));
        }

        return new SocketOperationResult(BytesTransferred);
    }

    public ValueTaskSourceStatus GetStatus(short token)
    {
        if(!ReferenceEquals(_continuation, _continuationCompleted))
        {
            return ValueTaskSourceStatus.Pending;
        }
        else if(SocketError == SocketError.Success)
        {
            return ValueTaskSourceStatus.Succeeded;
        }

        return ValueTaskSourceStatus.Faulted;
    }

    public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
    {
        UserToken = state;

        var prevContinuation = Interlocked.CompareExchange(ref _continuation, continuation, null);

        if(ReferenceEquals(prevContinuation, _continuationCompleted))
        {
            UserToken = null;

            ThreadPool.UnsafeQueueUserWorkItem(continuation, state, preferLocal: true);
        }
    }

    protected override void OnCompleted(SocketAsyncEventArgs e)
    {
        var continuation = _continuation;

        if(continuation != null
            || (continuation = Interlocked.CompareExchange(ref _continuation, _continuationCompleted, null)) != null)
        {
            var state = UserToken;
            UserToken = null;
            _continuation = _continuationCompleted;

            _scheduler.Schedule(continuation, state);
        }
    }

    protected static SocketException CreateException(SocketError error)
    {
        return new SocketException((int)error);
    }
}
