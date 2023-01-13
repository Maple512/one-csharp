namespace OneI.Logable;

public interface ILoggerMiddleware
{
    void Invoke(in LoggerMessageContext context);
}
