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

    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        List<Exception>? exceptions = null;
        foreach(var middleware in _middlewares)
        {
            try
            {
                // 平行的中间件，无效的next
                middleware.Invoke(context, ILoggerMiddleware.Nullable);
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
            ThrowAggregateException(exceptions);
        }

        return next(context);

        [DoesNotReturn]
        static void ThrowAggregateException(IEnumerable<Exception> exceptions)
        {
            throw new AggregateException(exceptions);
        }
    }
}
