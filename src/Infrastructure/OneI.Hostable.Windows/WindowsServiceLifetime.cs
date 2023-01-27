namespace OneI.Hostable;

using System.ServiceProcess;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class WindowsServiceLifetime : ServiceBase, IHostLifetime
{
    // 表示未绑定到委托的 Task<TResult> 的制造者方，并通过 Task 属性提供对使用者方的访问
    private readonly TaskCompletionSource<object?> _delayStart = new(TaskCreationOptions.RunContinuationsAsynchronously);
    // 表示线程同步事件，收到信号时，必须手动重置该事件。 此类是 ManualResetEvent 的轻量替代项
    private readonly ManualResetEventSlim _delayStop = new();

    private readonly IHostEnvironment _environment;
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly HostOptions _hostOptions;

    public WindowsServiceLifetime(
        IHostEnvironment environment,
        ILoggerFactory loggerFactory,
        IHostApplicationLifetime hostApplicationLifetime,
        IOptions<HostOptions> hostOptions,
        IOptions<WindowsServiceLifetimeOptions> options)
    {
        _environment = environment;
        _logger = loggerFactory.CreateLogger<IHostLifetime>();
        _hostApplicationLifetime = hostApplicationLifetime;
        _hostOptions = hostOptions.Value;

        CanShutdown = true;
        ServiceName = options.Value.ServiceName;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => _delayStart.TrySetCanceled());

        _hostApplicationLifetime.Started.Register(() =>
        {
            _logger.LogInformation("The host started.");
            _logger.LogInformation($"Environment: {_environment.EnvironmentName}");
            _logger.LogInformation($"Root: {_environment.RootPath}");
        });

        _hostApplicationLifetime.Stopping.Register(() =>
        {
            _logger.LogInformation("The host is shutting down...");
        });

        var thread = new Thread(Run)
        {
            IsBackground = true,
        };

        thread.Start();

        return _delayStart.Task;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Task.Run(Stop, CancellationToken.None);

        return Task.CompletedTask;
    }

    private void Run()
    {
        try
        {
            Run(this);
            _delayStart.TrySetException(new InvalidOperationException("Stopped without starting."));
        }
        catch(Exception ex)
        {
            _delayStart.TrySetException(ex);
        }
    }

    protected override void OnStart(string[] args)
    {
        _delayStart.TrySetResult(null);

        base.OnStart(args);
    }

    protected override void OnStop()
    {
        _hostApplicationLifetime.StopApplication();
        _delayStop.Wait(_hostOptions.ShutdownTimeout);
        base.OnStop();
    }

    protected override void OnShutdown()
    {
        _hostApplicationLifetime.StopApplication();
        _delayStop.Wait(_hostOptions.ShutdownTimeout);
        base.OnShutdown();
    }

    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            _delayStop.Set();
        }

        base.Dispose(disposing);
    }
}
