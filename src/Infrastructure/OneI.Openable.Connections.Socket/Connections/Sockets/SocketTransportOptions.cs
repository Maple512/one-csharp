namespace OneI.Openable.Connections.Sockets;

using System.Net;
using System.Net.Sockets;

public sealed class SocketTransportOptions
{
    /// <summary>
    /// 用于处理请求的队列长度
    /// </summary>
    /// <remarks>
    /// 默认值为 <see cref="Environment.ProcessorCount" /> 向下舍入，[1,16]
    /// </remarks>
    public int IOQueueCount { get; set; } = Math.Min(Environment.ProcessorCount, 16);

    /// <summary>
    /// 在分配缓冲区之前等待数据。
    /// 将此设置为<see langword="false"/>会通过增加内存来增加吞吐量。
    /// </summary>
    /// <remarks>
    /// 默认为<see langword="true"/>
    /// </remarks>
    public bool WaitForDataBeforeAllocatingBuffer { get; set; } = true;

    /// <summary>
    /// 设置为<see langword="false"/>可为所有链接启用Nagle算法
    /// </summary>
    /// <remarks>
    /// <para>默认为<see langword="true"/></para>
    /// <para>Nagle算法：通过减少发送数据包的数量来提高TCP/IP传输效率。主要避免发送较小的数据包</para>
    /// </remarks>
    public bool NoDelay { get; set; } = true;

    /// <summary>
    /// 等待链接队列的最大长度
    /// </summary>
    /// <remarks>
    /// 默认值为<see langword="512" />
    /// </remarks>
    public int WaitingConnectionQueueMaxLength { get; set; } = 512;

    /// <summary>
    /// 最大读取缓冲长度
    /// <para>
    /// 设置为<see langword="null"/>或<see langword="0"/>将禁用缓冲。
    /// 对于不授信任的客户端，禁用缓冲是一种安全风险。
    /// </para>
    /// </summary>
    /// <remarks>
    /// 默认为 1 MiB
    /// </remarks>
    public long? MaxReadBufferLength { get; set; } = 1024 * 1024;

    /// <summary>
    /// 最大写入缓冲长度
    /// <para>
    /// 设置为<see langword="null"/>或<see langword="0"/>将禁用缓冲。
    /// 对于不授信任的客户端，禁用缓冲是一种安全风险。
    /// </para>
    /// </summary>
    /// <remarks>
    /// 默认为 64 KiB
    /// </remarks>
    public long? MaxWriteBufferLength { get; set; } = 64 * 1024;

    /// <summary>
    /// 内联应用程序和传输继续，而不是分派到线程池。
    /// </summary>
    /// <remarks>
    /// 这将在IO线程上运行应用程序代码，这就是为什么这是不安全的。
    /// 当使用此设置同时在运行时层内联完成时，建议将DOTNET_SYSTEM_NET_SOCKETS_INLINE_COMPLETIONS环境变量设置为“1”。
    /// 如果有昂贵的工作最终会占用IO线程超过所需的时间，则此设置可能会使性能变差。
    /// 测试以确保此设置有助于性能。
    /// </remarks>
    /// <remarks>
    /// 默认为 <see langword="false"/>
    /// </remarks>
    public bool UnsafePreferInlineScheduler { get; set; }

    /// <summary>
    /// A function used to create a new <see cref="Socket"/> to listen with. If
    /// not set, <see cref="CreateDefaultBoundListenSocket" /> is used.
    /// </summary>
    /// <remarks>
    /// Implementors are expected to call <see cref="Socket.Bind"/> on the
    /// <see cref="Socket"/>. Please note that <see cref="CreateDefaultBoundListenSocket"/>
    /// calls <see cref="Socket.Bind"/> as part of its implementation, so implementors
    /// using this method do not need to call it again.
    /// </remarks>
    /// <remarks>
    /// Defaults to <see cref="CreateDefaultBoundListenSocket"/>.
    /// </remarks>
    public Func<EndPoint, Socket> CreateBoundListenSocket { get; set; } = CreateDefaultBoundListenSocket;

    /// <summary>
    /// 为给定的<see cref="EndPoint"/>创建一个默认实例。
    /// 连接监听器可以使用该实例侦听入站请求 <see cref="Socket.Bind"/>。
    /// </summary>
    public static Socket CreateDefaultBoundListenSocket(EndPoint endpoint)
    {
        Socket listenSocket;
        switch(endpoint)
        {
            case FileHandleEndPoint fileHandle:
                // 我们传递“ownsHandle:false”以避免在处理套接字时对句柄产生副作用。
                // 当 non-owning SafeSocketHandle被释放（在.NET 7+上）时，正在进行的异步操作将中止。
                listenSocket = new Socket(new SafeSocketHandle((IntPtr)fileHandle.Handle, ownsHandle: false));
                break;
            case UnixDomainSocketEndPoint unix:
                listenSocket = new Socket(unix.AddressFamily, SocketType.Stream, ProtocolType.Unspecified);
                break;
            case IPEndPoint ip:
                listenSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Kestrel expects IPv6Any to bind to both IPv6 and IPv4
                if(ip.Address.Equals(IPAddress.IPv6Any))
                {
                    listenSocket.DualMode = true;
                }

                break;
            default:
                listenSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                break;
        }

        // 我们只对使用文件句柄创建的套接字调用Bind；
        // 句柄已经绑定到基础套接字，因此再次这样做会导致基础PAL调用抛出
        if(endpoint is not FileHandleEndPoint)
        {
            listenSocket.Bind(endpoint);
        }

        return listenSocket;
    }
}
