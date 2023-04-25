namespace OneI.Openable.Connections;

using System.Net;

public sealed class FileHandleEndPoint : EndPoint
{
    public FileHandleEndPoint(ulong handle, FileHandleType type)
    {
        if((int)type > 2)
        {
            throw new NotSupportedException();
        }

        Handle = handle;
        Type = type;
    }

    public ulong Handle { get; }

    public FileHandleType Type { get; }
}
