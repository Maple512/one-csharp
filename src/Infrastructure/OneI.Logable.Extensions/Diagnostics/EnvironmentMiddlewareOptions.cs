namespace OneI.Logable.Diagnostics;

public class EnvironmentOptions
{
    /// <summary>
    /// 该进程的命令行，属性名：<see cref="Environment.CommandLine"/>
    /// </summary>
    public bool HasCommandLine { get; init; }

    /// <summary>
    /// 当前工作目录的完全限定路径，属性名：<see cref="Environment.CurrentDirectory"/>
    /// </summary>
    public bool HasCurrentDirectory { get; init; }

    /// <summary>
    /// 托管线程的唯一标识符，属性名：<see cref="Environment.CurrentManagedThreadId"/>
    /// </summary>
    public bool HasCurrentManagedThreadId { get; init; }

    /// <summary>
    /// 值指示当前操作系统是否为 64 位操作系统，属性名：<see cref="Environment.Is64BitOperatingSystem"/>
    /// </summary>
    public bool HasIs64BitOperatingSystem { get; init; }

    /// <summary>
    /// 该值指示当前进程是否为 64 位进程，属性名：<see cref="Environment.Is64BitProcess"/>
    /// </summary>
    public bool HasIs64BitProcess { get; init; }

    /// <summary>
    /// 此本地计算机的 NetBIOS 名称，属性名：<see cref="Environment.MachineName"/>
    /// </summary>
    public bool HasMachineName { get; init; }

    /// <summary>
    /// 当前平台标识符和版本号，属性名：<see cref="Environment.OSVersion"/>
    /// </summary>
    public bool HasOSVersion { get; init; }

    /// <summary>
    /// 当前进程的唯一标识符，属性名：<see cref="Environment.ProcessId"/>
    /// </summary>
    public bool HasProcessId { get; init; }

    /// <summary>
    /// 当前进程可用的处理器数，属性名：<see cref="Environment.ProcessorCount"/>
    /// </summary>
    public bool HasProcessorCount { get; init; }

    /// <summary>
    /// 启动当前正在执行的进程的可执行文件的路径，属性名：<see cref="Environment.ProcessPath"/>
    /// </summary>
    public bool HasProcessPath { get; init; }

    /// <summary>
    /// 与当前线程相关联的用户的用户名，属性名：<see cref="Environment.UserName"/>
    /// </summary>
    public bool HasUserName { get; init; }

    /// <summary>
    /// .NET 框架，属性名：<see cref="RuntimeInformation.FrameworkDescription"/>
    /// </summary>
    public bool HasFrameworkDescription { get; init; }
}
