namespace OneI.Openable.Connections;

public interface IConnectionBuilder
{
    IServiceProvider ServiceProvider { get; }

    IConnectionBuilder Use(Func<ConnectionDelegate, ConnectionDelegate> middleware);

    ConnectionDelegate Build();
}
