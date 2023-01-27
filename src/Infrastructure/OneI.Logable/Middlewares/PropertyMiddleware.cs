namespace OneI.Logable.Middlewares;

using Templatizations;

public class PropertyMiddleware<T> : ILoggerMiddleware
{
    private readonly string? _name;
    private readonly T? _value;
    private readonly IPropertyValueFormatter<T?>? _formatter;
    private readonly bool _addOrUpdate;

    public PropertyMiddleware(string? name, T? value, IPropertyValueFormatter<T?>? formatter = null, bool addOrUpdate = false)
    {
        _name = name;
        _value = value;
        _formatter = formatter;
        _addOrUpdate = addOrUpdate;
    }

    public void Invoke(LoggerMessageContext context)
    {
        if(_name is not { Length: > 0 })
        {
            return;
        }

        if(_addOrUpdate)
        {
            context.AddOrUpdateProperty(_name, _value, _formatter);
        }
        else
        {
            context.AddProperty(_name, _value, _formatter);
        }
    }
}
