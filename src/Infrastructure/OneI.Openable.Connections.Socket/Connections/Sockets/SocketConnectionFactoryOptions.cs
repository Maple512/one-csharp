namespace OneI.Openable.Connections.Sockets;

public sealed class SocketConnectionFactoryOptions
{
    public SocketConnectionFactoryOptions() { }

    internal SocketConnectionFactoryOptions(SocketTransportOptions connectionOptions)
    {
        IOQueueCount = connectionOptions.IOQueueCount;
        WaitForDataBeforeAllocatingBuffer = connectionOptions.WaitForDataBeforeAllocatingBuffer;
        ReadBufferMaxLength = connectionOptions.MaxReadBufferLength;
        WriteBufferMaxLength = connectionOptions.MaxWriteBufferLength;
        UnsafePreferInlineScheduler = connectionOptions.UnsafePreferInlineScheduler;
    }

    /// <summary>
    /// 用于处理请求的I/O队列数。设置为0可直接调度ThreadPool的I/O。
    /// </summary>
    /// <remarks>
    /// 默认值为 <see cref="Environment.ProcessorCount" /> 向下舍入，[1,16]
    /// </remarks>
    public int IOQueueCount { get; set; } = Math.Min(Environment.ProcessorCount, 16);

    /// <summary>
    /// 等待有数据可用于分配缓冲区。
    /// 将此设置为<see langword="false"/>会以增加内存使用为代价来增加吞吐量。
    /// </summary>
    /// <remarks>
    /// 默认值为<see langword="true"/>
    /// </remarks>
    public bool WaitForDataBeforeAllocatingBuffer { get; set; } = true;

    /// <summary>
    /// 最大读取缓冲区长度
    /// </summary>
    /// <remarks>
    /// 默认值为 1 MiB
    /// </remarks>
    public long? ReadBufferMaxLength { get; set; } = 1024 * 1024;

    /// <summary>
    /// 最大写入缓冲区长度
    /// </summary>
    /// <remarks>
    /// 默认值为 64 KiB
    /// </remarks>
    public long? WriteBufferMaxLength { get; set; } = 64 * 1024;

    /// <summary>
    /// 内联应用程序和传输延续，而不是分派到线程池。
    /// </summary>
    /// <remarks>
    /// 这将在IO线程上运行应用程序代码，这就是为什么这是不安全的。
    /// 当使用此设置同时在运行时层内联完成时，建议将<see langword="DOTNET_SYSTEM_NET_SOCKETS_INLINE_COMPLETIONS"/>环境变量设置为“1”。
    /// 如果有昂贵的工作最终会占用IO线程超过所需的时间，则此设置可能会使性能变差。
    /// 测试以确保此设置有助于性能。
    /// </remarks>
    public bool UnsafePreferInlineScheduler { get; set; }
}
