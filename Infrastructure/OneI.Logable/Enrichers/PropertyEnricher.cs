namespace OneI.Logable.Enrichers;

using OneI.Logable.Properties;

public class PropertyEnricher : ILoggerEnricher
{
    private readonly string _name;
    private readonly object? _value;
    private readonly bool _deconstruct;

    public PropertyEnricher(string name, object? value, bool deconstruct)
    {
        _name = name;
        _value = value;
        _deconstruct = deconstruct;
    }

    public void Enrich(LoggerContext context, IPropertyFactory propertyFactory)
    {
        var property = propertyFactory.Create(_name, _value, _deconstruct);

        context.AddPropertyIfAbsent(property);
    }
}
