namespace OneI.Hostable.Internal;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class Host : IHost, IAsyncDisposable
{
    private readonly ILogger<IHost> _logger;
    private readonly IHostLifetime _hostLifetime;
    private readonly HostApplicationLifetime _applicationLifetime;
    private readonly HostOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly PhysicalFileProvider _defaultProvider;
    private IEnumerable<IHostStrartingService>? _hostStartingServices;
    private volatile bool _stopCalled;

    public Host(
        ILogger<IHost> logger,
        IHostLifetime lifetime,
        IHostApplicationLifetime applicationLifetime,
        IOptions<HostOptions> options,
        IHostEnvironment environment,
        IServiceProvider services,
        PhysicalFileProvider defaultProvider)
    {
        _logger = logger;
        _hostLifetime = lifetime;
        if(applicationLifetime is not HostApplicationLifetime hostApplicationLifetime)
        {
            throw new ArgumentException($"Replacing {nameof(IHostApplicationLifetime)} is not supported.", nameof(applicationLifetime));
        }

        _applicationLifetime = hostApplicationLifetime;
        _options = options.Value;
        _environment = environment;
        Services = services;
        _defaultProvider = defaultProvider;
    }

    public IServiceProvider Services { get; }

    public void Dispose()
    {

        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {

        // The user didn't change the FileProvider instance, we can dispose it
        if(ReferenceEquals(_environment.FileProvider, _defaultProvider))
        {
            await DisposeAsync(_environment.FileProvider);
        }
        else
        {
            // In the rare case that the user replaced the FileProvider, dispose it and the one
            // we originally created
            await DisposeAsync(_environment.FileProvider);
            await DisposeAsync(_defaultProvider);
        }

        await DisposeAsync(Services);

        static async ValueTask DisposeAsync(object? o)
        {
            switch(o)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.HostStarting();

        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var token = tokenSource.Token;

        await _hostLifetime.StartAsync(token);

        token.ThrowIfCancellationRequested();

        _hostStartingServices = Services.GetRequiredService<IEnumerable<IHostStrartingService>>();

        foreach(var item in _hostStartingServices)
        {

        }

        _applicationLifetime.NotifyStarted();

        _logger.HostStarted();
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _stopCalled = true;
        _logger.HostStopping();

        using var tokenSource = new CancellationTokenSource(_options.ShutdownTimeout);
        using var likedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, cancellationToken);
        var token = likedTokenSource.Token;

        _applicationLifetime.StopApplication();

        var exceptions = new List<Exception>();
        if(_hostStartingServices is not null)
        {
            foreach(var service in _hostStartingServices.Reverse())
            {
                try
                {
                    await service.StopAsync(token);
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        }

        _applicationLifetime.NotifyStopped();

        try
        {
            await _hostLifetime.StopAsync(token);
        }
        catch(Exception ex)
        {
            exceptions.Add(ex);
        }

        if(exceptions.Count > 0)
        {
            var exception = new AggregateException(exceptions.ToArray());

            _logger.StoppedWithException(exception);

            throw exception;
        }

        _logger.HostStopped();
    }

    private async Task TryExecuteBackgroundServiceAsync(BackgroundService service)
    {
        var task = service.ExecuteTask;
        if(task == null)
        {
            return;
        }

        try
        {
            await task;
        }
        catch(Exception ex)
        {
            if(_stopCalled
                && task.IsCanceled
                && ex is OperationCanceledException)
            {
                return;
            }

            _logger.BackgroundServiceFaulted(ex);

            if(_options.BackgroundServiceExceptionBehavior == BackgroundServiceExceptionBehavior.StopHost)
            {
                _logger.BackgroundServiceStoppingHost(ex);
                _applicationLifetime.StopApplication();
            }
        }
    }
}
