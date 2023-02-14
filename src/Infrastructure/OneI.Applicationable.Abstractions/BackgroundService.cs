namespace OneI.Applicationable;

public abstract class BackgroundService : IApplicationStartingService, IDisposable
{
    private CancellationTokenSource? _stoppingToken;

    public virtual Task? ExecuteTask
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    }

    protected abstract Task ExecuteAsync(CancellationToken stopptingToken);

    public virtual Task StartAsync(CancellationToken cancellationToken = default)
    {
        // 链接到提供的取消令牌
        _stoppingToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        ExecuteTask = ExecuteAsync(_stoppingToken.Token);

        if(ExecuteTask.IsCompleted)
        {
            return ExecuteTask;
        }

        return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if(ExecuteTask is null)
        {
            return;
        }

        try
        {
            // 触发取消信号
            _stoppingToken!.Cancel();
        }
        finally
        {
            // 等待任务完成，或取消令牌触发
            var taskSource = new TaskCompletionSource<object>();

            using var registration = cancellationToken.Register(state =>
            {
                _ = ((TaskCompletionSource<object>)state!).TrySetCanceled();
            }, taskSource);

            _ = await Task.WhenAny(ExecuteTask, taskSource.Task);
        }
    }

    public void Dispose()
    {
        _stoppingToken?.Cancel();
    }
}
