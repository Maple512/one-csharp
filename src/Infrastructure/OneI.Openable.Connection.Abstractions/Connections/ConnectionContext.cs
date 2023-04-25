namespace OneI.Openable.Connections;

using System.IO.Pipelines;

public abstract class ConnectionContext : ConnectionContextBase
{
    public abstract IDuplexPipe Transport { get; set; }

    public override void Abort()
        => Abort(new ConnectionAbortedException($"The connection was aborted by the application via {nameof(ConnectionContext)}.{nameof(Abort)}()."));
}
