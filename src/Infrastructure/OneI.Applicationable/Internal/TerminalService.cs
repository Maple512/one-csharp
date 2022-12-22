namespace OneI.Applicationable;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
/// <summary>
/// The terminal service.
/// </summary>

internal class TerminalService : ITerminalService, IDisposable
{
    private CancellationTokenRegistration _applicationStartedRegistration;
    private CancellationTokenRegistration _applicationStoppingRegistration;

    private readonly IApplicationEnvironment _environment;
    private readonly IApplicationLifetimeService _applicationLifetimeService;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalService"/> class.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <param name="applicationLifetimeService">The application lifetime service.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public TerminalService(
        IApplicationEnvironment environment,
        IApplicationLifetimeService applicationLifetimeService,
        ILoggerFactory loggerFactory)
    {
        _environment = environment;
        _applicationLifetimeService = applicationLifetimeService;
        _logger = loggerFactory.CreateLogger("OneI.Applicationable.TerminalLifetimeService");
    }

    /// <summary>
    /// Waits the for start async.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask.</returns>
    public ValueTask WaitForStartAsync(CancellationToken cancellationToken)
    {
        _applicationStartedRegistration = _applicationLifetimeService.Started.Register(state =>
        {
            ((TerminalService)state!).OnStarted();
        }, this);

        _applicationStoppingRegistration = _applicationLifetimeService.Stopping.Register(state =>
        {
            ((TerminalService)state!).OnStopping();
        }, this);

        RegisterShutdownHandlers();

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Stops the async.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask.</returns>
    public ValueTask StopAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Disposes the.
    /// </summary>
    public void Dispose()
    {
        UnregisterShutdownHandlers();

        _applicationStartedRegistration.Dispose();
        _applicationStoppingRegistration.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Ons the started.
    /// </summary>
    private void OnStarted()
    {
        _logger.LogInformation("Application started. Press Ctrl+C to shut down.");
        _logger.LogInformation("Application: {App}", _environment.ApplicationName);
        _logger.LogInformation("Environment: {Environment}", _environment.EnvironmentName);
        _logger.LogInformation("Root path: {RootRoot}", _environment.RootPath);
    }

    /// <summary>
    /// Ons the stopping.
    /// </summary>
    private void OnStopping()
    {
        _logger.LogInformation("Application is shutting down...");
    }

    private PosixSignalRegistration? _sigIntRegistration;
    private PosixSignalRegistration? _sigQuitRegistration;
    private PosixSignalRegistration? _sigTermRegistration;

    /// <summary>
    /// 注册关于 关闭终端 的处理程序
    /// </summary>
    private void RegisterShutdownHandlers()
    {
        Action<PosixSignalContext> handler = HandlePosixSignal;

        // 中断
        _sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, handler);

        // 退出
        _sigQuitRegistration = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, handler);

        // 终止
        _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, handler);
    }

    /// <summary>
    /// 收到信号后的处理
    /// </summary>
    /// <param name="context"></param>
    private void HandlePosixSignal(PosixSignalContext context)
    {
        Debug.Assert(
            context.Signal is PosixSignal.SIGINT
            or PosixSignal.SIGQUIT
            or PosixSignal.SIGTERM);

        context.Cancel = true;

        _applicationLifetimeService.OnApplicationStopping();
    }

    /// <summary>
    /// 撤销关于 关闭终端 的处理程序
    /// </summary>
    private void UnregisterShutdownHandlers()
    {
        _sigIntRegistration?.Dispose();
        _sigQuitRegistration?.Dispose();
        _sigTermRegistration?.Dispose();
    }
}
