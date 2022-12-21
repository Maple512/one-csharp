namespace OneI.Applicationable;

using System;
using System.Threading;
using System.Threading.Tasks;

public abstract class BackgroundService : IApplicationPipelineService, IDisposable
{
    private CancellationTokenSource? _stoppingCancellationSource;

    private Task? _executeTask;
    public Task? ExecuteTask { get; private set; }

    /// <summary>
    /// 指示应用后台服务异常后的行为（默认：<see cref="BackgroundServiceExceptionBehavior.Stop"/>）
    /// </summary>
    public virtual BackgroundServiceExceptionBehavior ExceptionBehavior { get; protected set; } = BackgroundServiceExceptionBehavior.Stop;

    /// <summary>
    /// 应用正常启动后触发
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _stoppingCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _executeTask = ExecuteAsync(_stoppingCancellationSource.Token);

        if(_executeTask.IsCompleted)
        {
            return _executeTask;
        }

        return Task.CompletedTask;
    }

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    /// <summary>
    /// 应用正常终止时触发
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if(_executeTask == null)
        {
            await Task.CompletedTask;

            return;
        }

        try
        {
            _stoppingCancellationSource!.Cancel();
        }
        finally
        {
            await Task.WhenAny(_executeTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    public void Dispose()
    {
        _stoppingCancellationSource?.Cancel();

        GC.SuppressFinalize(this);
    }
}
