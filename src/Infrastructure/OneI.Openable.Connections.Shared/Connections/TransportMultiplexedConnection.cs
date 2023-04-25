namespace OneI.Openable.Connections;

using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using OneI.Generators;

public abstract class TransportMultiplexedConnection : MultiplexedConnectionContext
{
    private string? _id;

    public override EndPoint? LocalEndPoint { get; set; }

    public override EndPoint? RemoteEndPoint { get; set; }

    public override string ConnectionId
    {
        get => _id ??= CorrelationIdGenerator.NextId();
        set => _id = value;
    }

    public virtual MemoryPool<byte> MemoryPool { get; } = default!;

    public IDuplexPipe Server { get; set; } = default!;

    public override CancellationToken ClosedToken { get; set; }

    public override void Abort(ConnectionAbortedException reason)
    {
        Server.Input.CancelPendingRead();
    }

    public override void Abort()
    {
        Abort(new ConnectionAbortedException("The connection was aborted by the server via Abort()."));
    }
}
