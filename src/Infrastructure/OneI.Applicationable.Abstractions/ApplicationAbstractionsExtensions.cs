namespace OneI.Applicationable;

public static class ApplicationAbstractionsExtensions
{
    public static void Run(this IApplication application, CancellationToken token = default)
    {
        RunAsync(application, token).GetAwaiter().GetResult();
    }

    public static async Task RunAsync(this IApplication application, CancellationToken token = default)
    {
        try
        {
            await application.StartAsync(token);

            await application.StopAsync(token);
        }
        finally
        {
            if(application is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if(application is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
