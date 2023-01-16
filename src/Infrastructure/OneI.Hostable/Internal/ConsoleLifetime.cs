namespace OneI.Hostable.Internal;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class ConsoleLifetime : IHostLifetime, IDisposable
{
    private CancellationTokenRegistration _startedCallback;
    private CancellationTokenRegistration _stoppingCallback;

    private readonly HostLifetimeOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger _logger;

    public ConsoleLifetime(
        IOptions<HostLifetimeOptions> options,
        IHostEnvironment environment,
        IHostApplicationLifetime applicationLifetime,
        ILoggerFactory loggerFactory)
    {
        _options = options.Value;
        _environment = environment;
        _applicationLifetime = applicationLifetime;
        _logger = loggerFactory.CreateLogger<IHostLifetime>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if(_options.IsSuppressStatusMessages)
        {
            _startedCallback = _applicationLifetime.Started.Register(state =>
            {
                ((ConsoleLifetime)state!).OnStarted();
            }, this);

            _stoppingCallback = _applicationLifetime.Started.Register(state =>
            {
                ((ConsoleLifetime)state!).OnStopping();
            }, this);
        }

        RegisterShutdownHandlers();

        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        _logger.LogInformation("The host started. Press Ctrl+C to shut down.");
        _logger.LogInformation($"Environment: {_environment.EnvironmentName}");
        _logger.LogInformation($"Root path: {_environment.RootPath}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // 不需要做任何事
        return Task.CompletedTask;
    }

    private void OnStopping()
    {
        _logger.LogInformation("The host is shutting down...");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        UnregisterShutdownHandlers();

        _startedCallback.Dispose();
        _stoppingCallback.Dispose();
    }

    private PosixSignalRegistration? _sigIntHandler;// 中断
    private PosixSignalRegistration? _sigQuitHandler;// 退出
    private PosixSignalRegistration? _sigTermHandler;// 终止
    private void RegisterShutdownHandlers()
    {
        var handler = HandlePosixSignal;
        _sigIntHandler = PosixSignalRegistration.Create(PosixSignal.SIGINT, handler);
        _sigQuitHandler = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, handler);
        _sigTermHandler = PosixSignalRegistration.Create(PosixSignal.SIGTERM, handler);
    }

    private void UnregisterShutdownHandlers()
    {
        _sigIntHandler?.Dispose();
        _sigQuitHandler?.Dispose();
        _sigTermHandler?.Dispose();
    }

    private void HandlePosixSignal(PosixSignalContext context)
    {
        context.Cancel = true;

        _applicationLifetime.StopApplication();
    }
}
