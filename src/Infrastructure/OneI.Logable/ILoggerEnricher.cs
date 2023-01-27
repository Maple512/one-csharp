namespace OneI.Logable;

public interface ILoggerEnricher
{
    void Enrich(LoggerMessageContext context);
}
