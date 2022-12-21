namespace OneI.Logable.Temporaries;

/// <summary>
/// 临时中间件
/// </summary>
public class TemporaryMiddleware : ILoggerMiddleware
{
    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        var queue = TemporaryContainer.GetOrCreateQueue();

        foreach(var item in queue)
        {
            item.Invoke(context, next);
        }

        return next(context);
    }
}
