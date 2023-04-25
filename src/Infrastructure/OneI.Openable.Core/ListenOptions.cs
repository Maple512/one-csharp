namespace OneI.Openable;

using System;
using System.Net;
using OneI.Openable.Connections;

public sealed class ListenOptions : IConnectionBuilder, IMultiplexedConnectionBuilder
{
    internal const HttpProtocols HttpProtocolDefault = HttpProtocols.Http1_2_3;

    List<Func<ConnectionDelegate, ConnectionDelegate>> _middleware = new();

    List<Func<MultiplexedConnectionDelegate, MultiplexedConnectionDelegate>> _multiplexedMiddleware = new();
    HttpProtocols _protocols = HttpProtocolDefault;

    public EndPoint EndPoint { get; internal set; }

    internal EndpointConfig

    public IServiceProvider ServiceProvider { get; }

    public MultiplexedConnectionDelegate Build()
    {
        throw new NotImplementedException();
    }

    public IMultiplexedConnectionBuilder Use(Func<MultiplexedConnectionDelegate, MultiplexedConnectionDelegate> middleware)
    {
        throw new NotImplementedException();
    }

    public IConnectionBuilder Use(Func<ConnectionDelegate, ConnectionDelegate> middleware)
    {
        throw new NotImplementedException();
    }

    ConnectionDelegate IConnectionBuilder.Build()
    {
        throw new NotImplementedException();
    }
}
