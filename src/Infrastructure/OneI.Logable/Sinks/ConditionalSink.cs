namespace OneI.Logable.Sinks;

using System;

/// <summary>
/// The conditional sink.
/// </summary>
public class ConditionalSink : ILoggerSink
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly Action<LoggerContext> _sink;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalSink"/> class.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="sink">The sink.</param>
    public ConditionalSink(Func<LoggerContext, bool> condition, Action<LoggerContext> sink)
    {
        _condition = condition;
        _sink = sink;
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Invoke(in LoggerContext context)
    {
        if(_condition(context))
        {
            _sink.Invoke(context);
        }
    }
}
