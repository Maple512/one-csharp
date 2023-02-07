namespace OneI.Logable;

public class TraceSourceOptions
{
    internal TraceSourceOptions(string sourceName, string? defaultSwitchValue = null)
    {
        Switch = new SourceSwitch(sourceName, defaultSwitchValue ?? string.Empty);
    }

    public SourceSwitch Switch { get; }

    public TraceListener? Listener { get; set; }
}
