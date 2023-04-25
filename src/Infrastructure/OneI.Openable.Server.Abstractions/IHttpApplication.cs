namespace OneI.Openable;

public interface IHttpApplication<TContext>
    where TContext : notnull
{
    TContext CreateContext();

    Task ProcessRequestAsync(TContext context);

    void DisposeContext(TContext context, Exception? exception);
}
