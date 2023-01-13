namespace OneI.Logable.Middlewares;

public class AggregateMiddleware : ILoggerMiddleware
{
    private readonly ILoggerMiddleware[] _middlewares;
    private readonly bool _isSilent;

    public AggregateMiddleware(ILoggerMiddleware[] middlewares, bool isSilent = false)
    {
        _middlewares = middlewares;
        _isSilent = isSilent;
    }

    public void Invoke(in LoggerMessageContext context)
    {
        List<Exception>? exceptions = null;
        foreach(var middleware in _middlewares)
        {
            try
            {
                middleware.Invoke(context);
            }
            catch(Exception ex)
            {
                InternalLog.WriteLine($"Exception received ({middleware}): {ex}");

                if(!_isSilent)
                {
                    exceptions ??= new List<Exception>();
                    exceptions.Add(ex);
                }
            }
        }

        if(!_isSilent && exceptions != null)
        {
            ThrowHelper.ThrowAggregateException(exceptions);
        }
    }
}
