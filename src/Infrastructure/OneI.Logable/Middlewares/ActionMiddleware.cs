namespace OneI.Logable.Middlewares;

using OneI.Logable.Templates;

public class ActionMiddleware : ILoggerMiddleware
{
    private readonly Action<LoggerMessageContext> _action;

    public ActionMiddleware(Action<LoggerMessageContext> action) => _action = action;

    public void Invoke(in LoggerMessageContext context, ref PropertyDictionary properties) => _action.Invoke(context);
}
