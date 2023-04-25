namespace OneI.Openable;

public interface IServer : IDisposable
{
    Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        where TContext : notnull;

    Task StopAsync(CancellationToken cancellationToken);
}
