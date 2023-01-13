namespace OneI.Logable.Middlewares;

public class ActionMiddleware : ILoggerMiddleware
{
    private readonly Action<LoggerMessageContext> _action;

    public ActionMiddleware(Action<LoggerMessageContext> action)
    {
        _action = action;
    }

    public void Invoke(in LoggerMessageContext context)
    {
        _action.Invoke(context);
    }
}
