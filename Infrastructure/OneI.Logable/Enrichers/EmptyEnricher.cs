namespace OneI.Logable.Enrichers;

using OneI.Logable.Properties;

public class EmptyEnricher : ILoggerEnricher
{
    public void Enrich(LoggerContext context, IPropertyFactory propertyFactory)
    {

    }
}
