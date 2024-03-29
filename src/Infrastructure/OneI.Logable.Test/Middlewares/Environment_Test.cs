namespace OneI.Logable.Middlewares;

using OneI.Logable;
using OneI.Logable.Fakes;

public class Environment_Test
{
    [Fact]
    public void environment_values()
    {
        var logger = Fake.CreateLogger("{ProcessId}, {FrameworkDescription}, {CommandLine}{NewLine}",
                                       config =>
                                       {
                                           _ = config.WithEnvironment(cofigure =>
                                           {
                                               cofigure.HasProcessId = true;
                                               cofigure.HasFrameworkDescription = true;
                                               cofigure.HasCommandLine = true;
                                           });
                                       });

        logger.Information("information");
    }

    [Fact]
    public void print_environment_values()
    {
        _ = Environment
                .CommandLine; // 该进程的命令行 **\one-csharp\Infrastructure\OneI.Logable.Test\bin\Debug\net7.0\testhost.dll --port 3553 --endpoint 127.0.0.1:03553 --role client --parentprocessid 12860 --telemetryoptedin true
        _ = Environment
            .CurrentDirectory; // 当前工作目录的完全限定路径 **\one-csharp\Infrastructure\OneI.Logable.Test\bin\Debug\net7.0
        _ = Environment.CurrentManagedThreadId; // 托管线程的唯一标识符 14
        _ = Environment.ExitCode; // 进程的退出代码0
        _ = Environment.HasShutdownStarted; // 该值指示当前的应用程序域是否正在卸载或者公共语言运行时 (CLR) 是否正在关闭false
        _ = Environment.Is64BitOperatingSystem; // 该值指示当前操作系统是否为 64 位操作系统true
        _ = Environment.Is64BitProcess; // 该值指示当前进程是否为 64 位进程true
        _ = Environment.MachineName; // 此本地计算机的 NetBIOS 名称DESKTOP-QMCPFK5
        _ = Environment.OSVersion; // 当前平台标识符和版本号Microsoft Windows NT 10.0.19044.0
        _ = Environment.ProcessId; // 当前进程的唯一标识符9904
        _ = Environment.ProcessorCount; // 当前进程可用的处理器数12
        _ = Environment
            .ProcessPath; // 启动当前正在执行的进程的可执行文件的路径**\one-csharp\Infrastructure\OneI.Logable.Test\bin\Debug\net7.0\testhost.exe
        _ = Environment.StackTrace; // 当前的堆栈跟踪信息   at System.Environment.get_StackTrace()
        _ = Environment.SystemDirectory; // 系统目录的完全限定路径C:\WINDOWS\system32
        _ = Environment.SystemPageSize; // 操作系统的内存页的字节数4096
        _ = Environment.TickCount; // 系统启动后经过的毫秒数46688359
        _ = Environment.UserDomainName; // 与当前用户关联的网络域名DESKTOP-QMCPFK5
        _ = Environment.UserInteractive; // 用以指示当前进程是否在用户交互模式中运行true
        _ = Environment.UserName; // 与当前线程相关联的用户的用户名Administrator
        _ = Environment.Version; // 由公共语言运行时的主要、次要版本、内部版本和修订号组成的版本7.0.0
        _ = Environment.WorkingSet; // 映射到进程上下文的物理内存量112898048
                                    // 平台体系结构System.Runtime.InteropServices.Architecture.X64
        _ = RuntimeInformation.OSArchitecture;
        // 应用的进程架构System.Runtime.InteropServices.Architecture.X64
        _ = RuntimeInformation.ProcessArchitecture;
        // 平台win10-x64
        _ = RuntimeInformation.RuntimeIdentifier;
        // .NET .NET 7.0.0
        _ = RuntimeInformation.FrameworkDescription;
        // 应用的操作系统 Microsoft Windows 10.0.19044
        _ = RuntimeInformation.OSDescription;
    }
}
