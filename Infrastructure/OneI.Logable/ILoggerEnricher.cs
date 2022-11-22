namespace OneI.Logable;

using OneI.Logable.Properties;

public interface ILoggerEnricher
{
    void Enrich(LoggerContext context, IPropertyFactory propertyFactory);
}
