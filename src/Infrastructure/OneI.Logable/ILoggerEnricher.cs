namespace OneI.Logable;

public interface ILoggerEnricher
{
    void Enrich(in LoggerMessageContext context);
}
