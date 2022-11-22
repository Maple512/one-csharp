namespace OneI.Logable.Enrichers;

using System;
using System.Collections.Generic;
using OneI.Logable.Properties;

public class SafeAggregateEnricher : ILoggerEnricher
{
    private readonly IReadOnlyList<ILoggerEnricher> _enrichers;

    public SafeAggregateEnricher(IReadOnlyList<ILoggerEnricher> enrichers)
    {
        _enrichers = enrichers;
    }

    public void Enrich(LoggerContext context, IPropertyFactory propertyFactory)
    {
        foreach(var enricher in _enrichers)
        {
            try
            {
                enricher.Enrich(context, propertyFactory);
            }
            catch(Exception)
            {

            }
        }
    }
}
