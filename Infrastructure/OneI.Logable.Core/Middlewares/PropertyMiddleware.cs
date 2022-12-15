namespace OneI.Logable.Middlewares;

using OneI.Logable;
using OneI.Logable.Templating.Properties;

public class PropertyMiddleware : ILoggerMiddleware
{
    private readonly string _name;
    private readonly PropertyValue _value;

    public PropertyMiddleware(string name, PropertyValue value)
    {
        _name = name;
        _value = value;
    }

    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        context.AddPropertyIfAbsent(new(_name, _value));

        return next(context);
    }
}
