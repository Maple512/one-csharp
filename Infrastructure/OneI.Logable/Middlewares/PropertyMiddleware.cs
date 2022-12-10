namespace OneI.Logable.Middlewares;

using OneI.Logable.Templating.Properties;

public class PropertyMiddleware : ILoggerMiddleware
{
    readonly string _name;
    readonly PropertyValue _value;

    public PropertyMiddleware(string name, PropertyValue value)
    {
        _name=name;
        _value=value;
    }

    public void Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        context.AddPropertyIfAbsent(new(_name, _value));

        next(context);
    }
}
