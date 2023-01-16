namespace OneI.Hostable;

using Microsoft.Extensions.DependencyInjection;

public static class HostAbstractionsExtensions
{
    /// <summary>
    /// Starts the host synchronously.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> to start.</param>
    public static void Start(this IHost host)
    {
        host.StartAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Attempts to gracefully stop the host with the given timeout.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> to stop.</param>
    /// <param name="timeout">The timeout for stopping gracefully. Once expired the
    /// server may terminate any remaining active connections.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public static async Task StopAsync(this IHost host, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        await host.StopAsync(cts.Token).ConfigureAwait(false);
    }

    /// <summary>
    /// Block the calling thread until shutdown is triggered via Ctrl+C or SIGTERM.
    /// </summary>
    /// <param name="host">The running <see cref="IHost"/>.</param>
    public static void WaitForShutdown(this IHost host)
    {
        host.WaitForShutdownAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Runs an application and block the calling thread until host shutdown.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> to run.</param>
    public static void Run(this IHost host)
    {
        host.RunAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Runs an application and returns a <see cref="Task"/> that only completes when the token is triggered or shutdown is triggered.
    /// The <paramref name="host"/> instance is disposed of after running.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> to run.</param>
    /// <param name="token">The token to trigger shutdown.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public static async Task RunAsync(this IHost host, CancellationToken token = default)
    {
        try
        {
            await host.StartAsync(token).ConfigureAwait(false);

            await host.WaitForShutdownAsync(token).ConfigureAwait(false);
        }
        finally
        {
            if(host is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                host.Dispose();
            }
        }
    }

    /// <summary>
    /// 返回通过给定令牌触发关闭时完成的任务
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task WaitForShutdownAsync(this IHost host, CancellationToken cancellationToken = default)
    {
        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

        cancellationToken.Register(state =>
        {
            Debugger.Break();
            ((IHostApplicationLifetime)state!).StopApplication();
        }, lifetime);

        var waitForStop = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        lifetime.Stopping.Register(state =>
        {
            Debugger.Break();
            ((TaskCompletionSource<object?>)state!).TrySetResult(null);
        }, waitForStop);

        await waitForStop.Task;

        // 取消令牌可能已被触发以取消阻止waitForStop。不要在这里传递它，因为这会触发失败的关机。
        await host.StopAsync(CancellationToken.None);
    }
}
