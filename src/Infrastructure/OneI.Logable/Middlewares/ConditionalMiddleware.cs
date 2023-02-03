namespace OneI.Logable.Middlewares;

using OneI.Logable.Templates;

public class ConditionalMiddleware : ILoggerMiddleware
{
    private readonly Func<LoggerMessageContext, bool> _condition;
    private readonly ILoggerMiddleware _middleware;

    public ConditionalMiddleware(Func<LoggerMessageContext, bool> condition, ILoggerMiddleware middleware)
    {
        _condition = condition;
        _middleware = middleware;
    }

    public void Invoke(in LoggerMessageContext context, ref PropertyDictionary properties)
    {
        if(_condition(context))
        {
            _middleware.Invoke(context, ref properties);
        }
    }
}
