namespace OneI.Openable.Connections;

/// <summary>
/// 表示链接中止
/// </summary>
public sealed class ConnectionAbortedException : OperationCanceledException
{
    public ConnectionAbortedException()
        : this("The connection was aborted")
    {
    }

    public ConnectionAbortedException(string message) : base(message)
    {
    }

    public ConnectionAbortedException(string message, Exception inner) : base(message, inner)
    {
    }
}
