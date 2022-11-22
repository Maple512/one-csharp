namespace OneI.Logable.Enrichers;

using System;
using OneI.Logable.Properties;

public class FixedPropertyEnricher : ILoggerEnricher
{
    private readonly Property _property;

    public FixedPropertyEnricher(in Property property)
    {
        if(property == default)
        {
            throw new ArgumentException(nameof(property));
        }

        _property = property;
    }

    public void Enrich(LoggerContext context, IPropertyFactory propertyFactory)
    {
        context.AddPropertyIfAbsent(_property);
    }
}
