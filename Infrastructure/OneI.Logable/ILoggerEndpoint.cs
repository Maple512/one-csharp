namespace OneI.Logable;

public interface ILoggerEndpoint
{
    void Invoke(in LoggerContext context);
}
