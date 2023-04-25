namespace OneI.Openable.Connections;

using System.Net;

public interface IConnectionListenerFactorySelector
{
    bool CanBind(EndPoint endpoint);
}
