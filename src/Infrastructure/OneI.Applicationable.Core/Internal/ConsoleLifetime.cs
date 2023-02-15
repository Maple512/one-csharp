namespace OneI.Applicationable.Internal;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OneI.Logable;

internal sealed class ConsoleLifetime : IApplicationHostLifetime, IDisposable
{
    private CancellationTokenRegistration _started;
    private CancellationTokenRegistration _stopping;

    private PosixSignalRegistration? _sigIntRegistration;
    private PosixSignalRegistration? _sigQuitRegistration;
    private PosixSignalRegistration? _sigTermRegistration;

    public ConsoleLifetime(
        IOptions<ConsoleLifetimeOptions> options,
        IApplicationEnvironment environment,
        IApplicationLifetime applicationLifetime,
        ILogger logger)
    {
        Options = options.Value;
        Environment = environment;
        ApplicationLifetime = applicationLifetime;
        Logger = logger.ForContext("OneI.Applicationable.Lifetime");
    }

    private ConsoleLifetimeOptions Options { get; }
    private IApplicationEnvironment Environment { get; }
    private IApplicationLifetime ApplicationLifetime { get; }
    private ILogger Logger { get; }

    public Task StartAsync(CancellationToken token)
    {
        if(!Options.SuppressStatusMessages)
        {
            _started = ApplicationLifetime.Started.Register(state =>
            {
                ((ConsoleLifetime)state!).OnApplicationStarted();
            }, this);

            _stopping = ApplicationLifetime.Stopping.Register(state =>
            {
                ((ConsoleLifetime)state!).OnApplicationStopping();
            }, this);
        }

        RegisterShutdownHandlers();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        UnregisterShutdownHandlers();

        _started.Dispose();
        _stopping.Dispose();
    }

    private void OnApplicationStarted()
    {
        Logger.Information("Application started. Press Ctrl+C to shut down.");
        Logger.Information($"Environment: {Environment.EnvironmentName}");
        Logger.Information($"Root path: {Environment.RootPath}");
    }

    private void OnApplicationStopping()
    {
        Logger.Information("Application is shutting down...");
    }

    private void RegisterShutdownHandlers()
    {
        Action<PosixSignalContext> handler = HandlePosixSignal;

        _sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, handler);
        _sigQuitRegistration = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, handler);
        _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, handler);
    }

    private void HandlePosixSignal(PosixSignalContext context)
    {
        context.Cancel = true;

        ApplicationLifetime.Stop();
    }

    private void UnregisterShutdownHandlers()
    {
        _sigIntRegistration?.Dispose();
        _sigQuitRegistration?.Dispose();
        _sigTermRegistration?.Dispose();
    }
}
