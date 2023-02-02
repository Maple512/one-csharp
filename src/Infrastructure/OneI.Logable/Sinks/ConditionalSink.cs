namespace OneI.Logable.Sinks;

public class ConditionalSink : ILoggerSink
{
    private readonly Func<LoggerContext,bool> _condition;
    private readonly ILoggerSink     _sink;

    public ConditionalSink(Func<LoggerContext,bool> condition, ILoggerSink sink)
    {
        _condition = condition;
        _sink      = sink;
    }

    public void Invoke(in LoggerContext context)
    {
        if(_condition(context))
        {
            _sink.Invoke(context);
        }
    }
}
