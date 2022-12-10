namespace OneI.Logable.Middlewares;

public interface ILoggerMiddleware
{
    void Invoke(in LoggerContext context, in LoggerDelegate next);
}
