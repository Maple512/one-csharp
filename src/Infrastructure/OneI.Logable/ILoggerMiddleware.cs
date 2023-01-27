namespace OneI.Logable;

public interface ILoggerMiddleware
{
    void Invoke(LoggerMessageContext context);
}
