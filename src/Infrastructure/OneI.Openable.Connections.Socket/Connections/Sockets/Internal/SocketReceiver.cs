namespace OneI.Openable.Connections.Sockets.Internal;

using System.IO.Pipelines;
using System.Net.Sockets;

internal sealed class SocketReceiver : SocketAwaitableEventArgs
{
    public SocketReceiver(PipeScheduler scheduler) : base(scheduler)
    {
    }

    public ValueTask<SocketOperationResult> WaitForDataAsync(Socket socket)
    {
        SetBuffer(Memory<byte>.Empty);

        if(socket.ReceiveAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success
            ? new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred))
            : new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }

    public ValueTask<SocketOperationResult> ReceiveAsync(Socket socket, Memory<byte> buffer)
    {
        SetBuffer(buffer);

        if(socket.ReceiveAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success
            ? new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred))
            : new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }
}
