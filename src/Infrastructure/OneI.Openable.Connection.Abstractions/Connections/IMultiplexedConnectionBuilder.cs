namespace OneI.Openable.Connections;

public interface IMultiplexedConnectionBuilder
{
    IServiceProvider ServiceProvider { get; }

    IMultiplexedConnectionBuilder Use(Func<MultiplexedConnectionDelegate, MultiplexedConnectionDelegate> middleware);

    MultiplexedConnectionDelegate Build();
}
