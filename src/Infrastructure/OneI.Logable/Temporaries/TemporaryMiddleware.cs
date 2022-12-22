namespace OneI.Logable.Temporaries;

/// <summary>
/// 临时中间件
/// </summary>
public class TemporaryMiddleware : ILoggerMiddleware
{
    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <returns>A LoggerVoid.</returns>
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
