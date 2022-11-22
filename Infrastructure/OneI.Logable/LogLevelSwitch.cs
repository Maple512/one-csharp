namespace OneI.Logable;

public struct LogLevelSwitch
{
    public const LogLevel Minimum = LogLevel.Verbose;

    public const LogLevel Maximum = LogLevel.Fatal;

    private volatile LogLevel _minimumLevel;

    public LogLevelSwitch(LogLevel minimumLevel)
    {
        MinimumLevel = minimumLevel;
    }

    public LogLevel MinimumLevel
    {
        get { return _minimumLevel; }
        set { _minimumLevel = value; }
    }
}
