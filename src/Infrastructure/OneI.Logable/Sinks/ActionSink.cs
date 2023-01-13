namespace OneI.Logable.Sinks;

public class ActionSink : ILoggerSink
{
    private readonly Action<LoggerContext> _action;

    public ActionSink(Action<LoggerContext> action)
    {
        _action = action;
    }

    public void Invoke(in LoggerContext context)
    {
        _action(context);
    }
}
