namespace OneI.Openable.Connections.Features;

public interface IConnectionHeartbeatFeature
{
    void OnHeartbeat<T>(Action<T> action, T state);
}
