namespace OneI.Atomicable;

public interface IRestorable
{
    Task Restore(CancellationToken cancellationToken = default);
}
