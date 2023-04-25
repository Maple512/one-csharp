namespace OneI.Openable.Connections.Quic.Internal;

using OneI.Logable;

internal sealed class QuicTransportContext
{
    public QuicTransportContext(ILogger logger, QuicTransportOptions options)
    {
        Logger = logger;
        Options = options;
    }

    public ILogger Logger { get; }

    public QuicTransportOptions Options { get; }
}
