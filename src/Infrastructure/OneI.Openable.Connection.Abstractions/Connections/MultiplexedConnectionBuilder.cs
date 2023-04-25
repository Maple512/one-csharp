namespace OneI.Openable.Connections;

using System;

public class MultiplexedConnectionBuilder : IMultiplexedConnectionBuilder
{
    private List<Func<MultiplexedConnectionDelegate, MultiplexedConnectionDelegate>> _components = new();

    public IServiceProvider ServiceProvider { get; }

    public MultiplexedConnectionBuilder(IServiceProvider services)
    {
        ServiceProvider = services;
    }

    public MultiplexedConnectionDelegate Build()
    {
        MultiplexedConnectionDelegate pipe = context => Task.CompletedTask;

        for(var i = _components.Count - 1; i >= 0; i--)
        {
            pipe = _components[i](pipe);
        }

        return pipe;
    }

    public IMultiplexedConnectionBuilder Use(Func<MultiplexedConnectionDelegate, MultiplexedConnectionDelegate> middleware)
    {
        _components.Add(middleware);

        return this;
    }
}
