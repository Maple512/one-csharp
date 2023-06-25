namespace OneI.Applicationable.Internal;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using OneI.Logable;

internal sealed class Application : IApplication, IDisposable, IAsyncDisposable
{
    private readonly ApplicationLifetime _lifetime;
    private readonly IApplicationHostLifetime _hostLifetime;
    private IEnumerable<IApplicationStartingService>? _startingServices;
    private readonly ApplicationOptions _options;
    private volatile bool _stopCalled;
    private readonly IFileProvider _fileProvider;

    internal Application(
        IServiceProvider services,
        IConfiguration configuration,
        IApplicationEnvironment environment,
        IApplicationLifetime lifetime,
        ILogger logger,
        IApplicationHostLifetime hostLifetime,
        ApplicationOptions options,
        IFileProvider fileProvider)
    {
        Services = services;
        Configuration = configuration;
        Environment = environment;
        _lifetime = (lifetime as ApplicationLifetime)!;
        Logger = logger;
        _hostLifetime = hostLifetime;
        _options = options;
        _fileProvider = fileProvider;
    }

    public IServiceProvider Services { get; }
    public IConfiguration Configuration { get; }
    public IApplicationEnvironment Environment { get; }
    public ILogger Logger { get; }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Logger.Debug("Application starting");

        using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Stopping);

        var combinedToken = combinedTokenSource.Token;

        await _hostLifetime.StartAsync(combinedToken);

        combinedToken.ThrowIfCancellationRequested();

        _startingServices = Services.GetRequiredService<IEnumerable<IApplicationStartingService>>();

        foreach(var item in _startingServices)
        {
            await item.StartAsync(combinedToken);

            if(item is BackgroundService bs)
            {
                _ = TryExecuteBackgroundServiceAsync(bs);
            }
        }

        _lifetime.NotifyStarted();

        Logger.Debug("Application started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _stopCalled = true;

        Logger.Debug("Application stopping");

        using var tokenSource = new CancellationTokenSource(_options.ShutdownTimeout);
        using(var linked = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, cancellationToken))
        {
            var token = linked.Token;

            _lifetime.Stop();

            List<Exception>? exceptions = null;
            if(_startingServices is not null)
            {
                foreach(var item in _startingServices.Reverse())
                {
                    try
                    {
                        await item.StopAsync(token);
                    }
                    catch(Exception ex)
                    {
                        (exceptions ?? new()).Add(ex);
                    }
                }
            }

            _lifetime.NotifyStopped();

            try
            {
                await _hostLifetime.StopAsync(token);
            }
            catch(Exception ex)
            {
                (exceptions ?? new()).Add(ex);
            }

            if(exceptions is not null)
            {
                var ex = new AggregateException(exceptions);

                Logger.Debug(ex, "Hosting shutdown exception.");

                throw ex;
            }
        }

        Logger.Debug("Application stopped.");
    }

    private async Task TryExecuteBackgroundServiceAsync(BackgroundService backgroundService)
    {
        // backgroundService.ExecuteTask may not be set (e.g. if the derived class doesn't call base.StartAsync)
        var backgroundTask = backgroundService.ExecuteTask;
        if(backgroundTask == null)
        {
            return;
        }

        try
        {
            await backgroundTask.ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            // When the host is being stopped, it cancels the background services.
            // This isn't an error condition, so don't log it as an error.
            if(_stopCalled && backgroundTask.IsCanceled && ex is OperationCanceledException)
            {
                return;
            }

            Logger.Error(ex, "BackgroundService failed.");
            if(_options.BackgroundServiceExceptionBehavior == BackgroundServiceExceptionBehavior.Stop)
            {
                Logger.Error(ex, "A BackgroundService has thrown an unhandled exception, and the IHost instance is stopping.");

                _lifetime.Stop();
            }
        }
    }

    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(Environment.FileProvider);

        if(!ReferenceEquals(Environment.FileProvider, _fileProvider))
        {
            await DisposeAsync(_fileProvider);
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
}
