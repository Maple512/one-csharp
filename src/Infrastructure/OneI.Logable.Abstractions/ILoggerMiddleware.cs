namespace OneI.Logable;

using OneI.Logable.Templates;

public interface ILoggerMiddleware
{
    void Invoke(in LoggerMessageContext context, ref PropertyDictionary properties);
}
