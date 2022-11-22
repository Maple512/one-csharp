namespace OneI.Logable.Enrichers;

using OneI.Logable.Properties;

public class ContainerEnricher : ILoggerEnricher
{
    public void Enrich(LoggerContext context, IPropertyFactory propertyFactory)
    {
        EnricherContainer.Enrich(context, propertyFactory);
    }
}
