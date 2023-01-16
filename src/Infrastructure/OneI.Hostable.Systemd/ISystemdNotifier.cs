namespace OneI.Hostable;

public interface ISystemdNotifier
{
    void Notify(ServiceState state);

    bool IsEnabled { get; }
}
