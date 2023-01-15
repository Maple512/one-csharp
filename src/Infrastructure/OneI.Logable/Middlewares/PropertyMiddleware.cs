namespace OneI.Logable.Middlewares;

using OneI.Logable.Templatizations;

public class PropertyMiddleware<T> : ILoggerMiddleware
{
    private readonly string? _name;
    private readonly T? _value;
    private readonly int? _index;
    private readonly IPropertyValueFormatter<T?>? _formatter;
    private readonly bool _addOrUpdate;

    public PropertyMiddleware(string? name, T? value, IPropertyValueFormatter<T?>? formatter = null, bool addOrUpdate = false)
    {
        _name = name;
        _value = value;
        _formatter = formatter;
        _addOrUpdate = addOrUpdate;
    }

    public PropertyMiddleware(int? index, T? value, IPropertyValueFormatter<T?>? formatter = null, bool addOrUpdate = false)
    {
        _index = index;
        _value = value;
        _formatter = formatter;
        _addOrUpdate = addOrUpdate;
    }

    public void Invoke(in LoggerMessageContext context)
    {
        if(_index.HasValue)
        {
            if(_addOrUpdate)
            {
                context.AddOrUpdateProperty(_index.Value, _value, _formatter);
            }
            else
            {
                context.AddProperty(_index.Value, _value, _formatter);
            }
        }
        else if(_name.NotNullOrEmpty())
        {
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
}
