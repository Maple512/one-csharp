namespace OneI.Openable.Connections;

using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using OneI.Generators;

internal class TransportConnection : ConnectionContext
{
    private IDictionary<object, object?>? _items;
    private string? _id;

    public override IDuplexPipe Transport { get; set; } = default!;

    public IDuplexPipe Application { get; set; } = default!;

    public override string ConnectionId
    {
        get => _id ??= CorrelationIdGenerator.NextId();
        set => _id = value;
    }

    public IDictionary<object, object?> Items
    {
        get => _items ??= new ConnectionItems();
        set => _items = value;
    }

    public virtual MemoryPool<byte> MemoryPool { get; } = default!;

    public virtual Socket? Socket { get; }

    public override EndPoint? LocalEndPoint { get; set; }

    public override EndPoint? RemoteEndPoint { get; set; }

    public virtual long Error { get; set; } = -1;

    public virtual bool CanRead { get; protected set; }

    public virtual bool CanWrite { get; protected set; }

    public virtual long StreamId { get; protected set; }

    public void AbortRead(long errorCode, ConnectionAbortedException abortReason) { }

    public void AbortWrite(long errorCode, ConnectionAbortedException abortReason) { }

    public void OnClosed(Action<object?> callback, object? state) { }

    public override void Abort(ConnectionAbortedException reason)
    {
        Application.Input.CancelPendingRead();
    }
}
