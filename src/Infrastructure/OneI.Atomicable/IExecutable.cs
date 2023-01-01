namespace OneI.Atomicable;

public interface IExecutable
{
    Task Execute(CancellationToken cancellationToken = default);
}
