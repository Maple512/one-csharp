namespace OneI.Logable.Middlewares;

using OneI.Logable.Templates;

public class PropertyMiddleware<T> : ILoggerMiddleware
{
    private readonly bool _addOrUpdate;
    private readonly string? _name;
    private readonly T? _value;

    public PropertyMiddleware(string? name, T? value, bool addOrUpdate = false)
    {
        _name = name;
        _value = value;
        _addOrUpdate = addOrUpdate;
    }

    public void Invoke(in LoggerMessageContext context, ref PropertyDictionary properties)
    {
        if(_name is not { Length: > 0, })
        {
            return;
        }

        if(_addOrUpdate)
        {
            properties.AddOrUpdate(_name, _value);
        }
        else
        {
            properties.Add(_name, _value);
        }
    }
}
