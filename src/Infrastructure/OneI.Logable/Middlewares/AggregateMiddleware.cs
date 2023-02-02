namespace OneI.Logable.Middlewares;

using OneI.Logable.Templates;

public class AggregateMiddleware : ILoggerMiddleware
{
    private readonly bool                           _isSilent;
    private readonly IEnumerable<ILoggerMiddleware> _middlewares;

    public AggregateMiddleware(IEnumerable<ILoggerMiddleware> middlewares, bool isSilent = false)
    {
        _middlewares = middlewares;
        _isSilent    = isSilent;
    }

    public void Invoke(in LoggerMessageContext context, ref PropertyDictionary properties)
    {
        List<Exception>? exceptions = null;
        foreach(var middleware in _middlewares)
        {
            try
            {
                middleware.Invoke(context, ref properties);
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
