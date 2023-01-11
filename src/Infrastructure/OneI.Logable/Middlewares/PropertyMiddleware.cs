namespace OneI.Logable.Middlewares;

using OneI.Logable.Templating.Properties;

/// <summary>
/// The property middleware.
/// </summary>
public class PropertyMiddleware : ILoggerMiddleware
{
    private readonly string _name;
    private readonly PropertyValue _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyMiddleware"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public PropertyMiddleware(string name, PropertyValue value)
    {
        _name = name;
        _value = value;
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <returns>A LoggerVoid.</returns>
    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        context.AddPropertyIfAbsent(_name, _value);

        return next(context);
    }
}
