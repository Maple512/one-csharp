namespace OneI.Openable.Connections.Sockets.Internal;

using System.Net.Sockets;

internal readonly struct SocketOperationResult
{
    public readonly SocketException? Error;

    /// <summary>
    /// 已传输字节数
    /// </summary>
    public readonly int TransferredBytes;

    public SocketOperationResult(SocketException error) : this()
    {
        ThrowHelper.ThrowIfNull(error);

        Error = error;
        TransferredBytes = 0;
    }

    public SocketOperationResult(int transferredBytes) : this()
    {
        TransferredBytes = transferredBytes;
        Error = null;
    }

    [MemberNotNullWhen(true, nameof(Error))]
    public readonly bool HasError
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Error is not null;
    }
}
