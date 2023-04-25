namespace OneI.Openable.Connections.Features;

public interface IStreamDirectionFeature
{
    bool CanRead { get; }

    bool CanWrite { get; }
}
