namespace OneI.Hostable;

using System.Net.Sockets;

public class SystemdNotifier : ISystemdNotifier
{
    private const string NOTIFY_SOCKET = nameof(NOTIFY_SOCKET);
    private readonly string? _socketPath;

    public SystemdNotifier()
        : this(GetNotifySocketPath())
    {
    }

    public SystemdNotifier(string? socketPath)
    {
        _socketPath = socketPath;
    }

    public bool IsEnabled => _socketPath is not null;

    public void Notify(ServiceState state)
    {
        if(IsEnabled)
        {
            using var socket = new Socket(AddressFamily.Unix, SocketType.Dgram, ProtocolType.Unspecified);

            var endpoint = new UnixDomainSocketEndPoint(_socketPath!);

            socket.Connect(endpoint);

            // 在这里进行非阻塞调用是安全的：这里发送的消息比内核缓冲区小得多，所以我们不会被阻塞
            socket.Send(state.GetDate());
        }
    }

    private static string? GetNotifySocketPath()
    {
        var socketPath = Environment.GetEnvironmentVariable(NOTIFY_SOCKET);
        if(socketPath is not { Length: > 0 })
        {
            return null;
        }

        if(socketPath[0] == '@')
        {
            socketPath = string.Create(socketPath.Length, socketPath, (buffer, state) =>
            {
                buffer[0] = char.MinValue;
                state.AsSpan(1).CopyTo(buffer[1..]);
            });
        }

        return socketPath;
    }
}
