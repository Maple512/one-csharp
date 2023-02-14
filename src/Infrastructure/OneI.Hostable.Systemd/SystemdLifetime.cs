namespace OneI.Hostable;

using Microsoft.Extensions.Logging;

public class SystemdLifetime : IHostLifetime, IDisposable
{
    private CancellationTokenRegistration _applicationStartedRegistration;
    private CancellationTokenRegistration _applicationStoppingRegistration;

    public SystemdLifetime(IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, ISystemdNotifier systemdNotifier, ILoggerFactory loggerFactory)
    {
        Environment = environment;
        HostApplicationLifetime = applicationLifetime;
        SystemdNotifier = systemdNotifier;
        Logger = loggerFactory.CreateLogger<IHostLifetime>();
    }

    private IHostApplicationLifetime HostApplicationLifetime { get; }

    private IHostEnvironment Environment { get; }

    private ILogger Logger { get; }

    private ISystemdNotifier SystemdNotifier { get; }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _applicationStartedRegistration = HostApplicationLifetime.Started.Register(state =>
        {
            ((SystemdLifetime)state!).OnApplicationStarted();
        },
        this);

        _applicationStoppingRegistration = HostApplicationLifetime.Stopping.Register(state =>
        {
            ((SystemdLifetime)state!).OnApplicationStopping();
        },
        this);

        RegisterShutdownHandlers();

        return Task.CompletedTask;
    }

    private void OnApplicationStarted()
    {
        Logger.LogInformation("The host started.");
        Logger.LogInformation($"Environment: {Environment.EnvironmentName}.");
        Logger.LogInformation($"Root: {Environment.RootPath}");

        SystemdNotifier.Notify(ServiceState.Started);
    }

    private void OnApplicationStopping()
    {
        Logger.LogInformation("The host is shutting down...");

        SystemdNotifier.Notify(ServiceState.Stopping);
    }

    public void Dispose()
    {
        UnregisterShutdownHandlers();

        _applicationStartedRegistration.Dispose();
        _applicationStoppingRegistration.Dispose();
    }

    private PosixSignalRegistration? _sigTermRegistration;

    private void RegisterShutdownHandlers()
    {
        // systemd只向服务进程发送SIGTERM，因此我们只侦听该信号。
        // 其他信号（例如SIGINT/SIGQUIT）将由默认的.NET运行时信号处理程序处理，不会导致系统服务正常关闭。
        // systemd only sends SIGTERM to the service process, so we only listen for that signal.
        // Other signals (ex. SIGINT/SIGQUIT) will be handled by the default .NET runtime signal handler
        // and won't cause a graceful shutdown of the systemd service.
        _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, HandlePosixSignal);
    }

    private void HandlePosixSignal(PosixSignalContext context)
    {
        Debug.Assert(context.Signal == PosixSignal.SIGTERM);

        context.Cancel = true;
        HostApplicationLifetime.StopApplication();
    }

    private void UnregisterShutdownHandlers()
    {
        _sigTermRegistration?.Dispose();
    }
}
