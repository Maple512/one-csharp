namespace OneI.Applicationable.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneI.Applicationable;

internal class Application : IApplication, IDisposable, IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly IApplicationLifetimeService _lifetimeService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITerminalService _terminalLifetimeService;
    private readonly ApplicationOptions _options;

    private IEnumerable<IApplicationPipelineService>? _applicationPipelineServices;

    private volatile bool _stopCalled;

    public Application(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _logger = serviceProvider.GetRequiredService<ILogger<Application>>();
        _lifetimeService = serviceProvider.GetRequiredService<IApplicationLifetimeService>();
        _terminalLifetimeService = serviceProvider.GetRequiredService<ITerminalService>();
        _options = serviceProvider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
    }

    public IServiceProvider ServiceProvider => _serviceProvider;

    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.Starting();

        // 合并两个通知
        var combinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetimeService.Stopping).Token;

        // 等待终端启动
        await _terminalLifetimeService.WaitForStartAsync(combinedCancellationToken);

        combinedCancellationToken.ThrowIfCancellationRequested();

        _applicationPipelineServices = _serviceProvider.GetServices<IApplicationPipelineService>();

        foreach(var pipeline in _applicationPipelineServices)
        {
            await pipeline.StartAsync(combinedCancellationToken);

            if(pipeline is BackgroundService backgroundService)
            {
                _ = TryExecuteBackgroundService(backgroundService);
            }
        }

        _lifetimeService.OnApplicationStarted();

        _logger.Started();
    }

    private async Task TryExecuteBackgroundService(BackgroundService service)
    {
        var task = service.ExecuteTask;
        if(task == null)
        {
            await Task.CompletedTask;

            return;
        }

        try
        {
            await task.ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            if(_stopCalled
                && task.IsCompleted
                && ex is OperationCanceledException)
            {
                return;
            }

            _logger.BackgroundServiceFaulted(ex);

            if(service.ExceptionBehavior == BackgroundServiceExceptionBehavior.Stop)
            {
                _logger.BackgroundServiceUnhandledException(ex);

                _lifetimeService.OnApplicationStopping();
            }
        }
    }

    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        _stopCalled = true;

        _logger.Stopping();

        using var cancellationTokenSource = new CancellationTokenSource(_options.ShutdownTimeoutSeconds);

        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, cancellationToken);

        var token = combinedCts.Token;

        _lifetimeService.OnApplicationStopping();

        var exceptions = new List<Exception>(_applicationPipelineServices?.GetCount() ?? 0 + 1);

        if(_applicationPipelineServices is not null)
        {
            foreach(var pipeline in _applicationPipelineServices.Reverse())
            {
                try
                {
                    await pipeline.StopAsync(token).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        }

        _lifetimeService.OnApplicationStopped();

        try
        {
            await _terminalLifetimeService.StopAsync(token).ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            exceptions.Add(ex);
        }

        if(exceptions.Count > 0)
        {
            var ex = new AggregateException("One or more hosted services failed to stop.", exceptions);

            _logger.StoppedWithException(ex);

            throw ex;
        }

        _logger.Stopped();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}
