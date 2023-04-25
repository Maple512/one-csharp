namespace OneI.Openable.Connections.Sockets.Internal;

using System.IO.Pipelines;

internal sealed class SocketSenderPool : IDisposable
{
    // REVIEW: 这确定可行吗？
    private const int MaxQueueLength = 1024;
    private ConcurrentQueue<SocketSender> _queue = new();
    private int _count;
    public readonly PipeScheduler Scheduler;
    private bool _disposed;

    public SocketSenderPool(PipeScheduler scheduler)
    {
        Scheduler = scheduler;
    }

    public SocketSender Rent()
    {
        if(_queue.TryDequeue(out var sender))
        {
            _ = Interlocked.Decrement(ref _count);

            return sender;
        }

        return new SocketSender(Scheduler);
    }

    public void Return(SocketSender sender)
    {
        if(_disposed || Interlocked.Increment(ref _count) > MaxQueueLength)
        {
            _ = Interlocked.Decrement(ref _count);

            sender.Dispose();

            return;
        }

        sender.Reset();

        _queue.Enqueue(sender);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
