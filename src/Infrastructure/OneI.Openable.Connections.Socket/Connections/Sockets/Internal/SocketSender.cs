namespace OneI.Openable.Connections.Sockets.Internal;

using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;

internal sealed class SocketSender : SocketAwaitableEventArgs
{
    private List<ArraySegment<byte>>? _buffers;

    public SocketSender(PipeScheduler scheduler) : base(scheduler)
    {
    }

    public ValueTask<SocketOperationResult> SendAsync(Socket socket, ReadOnlySequence<byte> buffer)
    {
        if(buffer.IsSingleSegment)
        {
            return SendAsync(socket, buffer.First);
        }

        SetBuffers(buffer);

        if(socket.SendAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var transferred = BytesTransferred;
        var error = SocketError;

        var result = error == SocketError.Success
            ? new SocketOperationResult(transferred)
            : new SocketOperationResult(CreateException(error));

        return new ValueTask<SocketOperationResult>(result);
    }

    public void Reset()
    {
        if(BufferList is not null)
        {
            BufferList = null;
            _buffers?.Clear();
        }
        else
        {
            SetBuffer(null, 0, 0);
        }
    }

    private ValueTask<SocketOperationResult> SendAsync(Socket socket, ReadOnlyMemory<byte> buffer)
    {
        SetBuffer(MemoryMarshal.AsMemory(buffer));

        if(socket.SendAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var transferred = BytesTransferred;
        var error = SocketError;

        var result = error == SocketError.Success
            ? new SocketOperationResult(transferred)
            : new SocketOperationResult(CreateException(error));

        return new ValueTask<SocketOperationResult>(result);
    }

    private void SetBuffers(ReadOnlySequence<byte> buffers)
    {
        _buffers ??= new((int)buffers.Length);

        var enumerator = buffers.GetEnumerator();
        while(enumerator.MoveNext())
        {
            if(MemoryMarshal.TryGetArray(enumerator.Current, out var array))
            {
                _buffers.Add(array);
            }
        }

        BufferList = _buffers;
    }
}
