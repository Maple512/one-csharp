namespace OneI.Hostable;

public readonly struct ServiceState
{
    private readonly byte[] _data;

    public static readonly ServiceState Started = new("READY=1");

    public static readonly ServiceState Stopping = new("STOPPING=1");

    public ServiceState(string state)
    {
        if(state is not { Length: > 0 })
        {
            throw new ArgumentNullException(nameof(state));
        }

        _data = Encoding.UTF8.GetBytes(state);
    }

    public override string ToString() => _data is null ? string.Empty : Encoding.UTF8.GetString(_data);

    internal byte[] GetDate() => _data;
}
