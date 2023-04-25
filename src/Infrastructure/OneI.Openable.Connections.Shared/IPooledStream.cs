namespace OneI.Openable;

internal interface IPooledStream
{
    long PoolExpirationTicks { get; }

    void DisposeCore();
}
