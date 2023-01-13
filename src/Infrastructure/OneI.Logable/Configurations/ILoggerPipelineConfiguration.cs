namespace OneI.Logable.Configurations;

using OneI.Logable;
using OneI.Logable.Middlewares;

public interface ILoggerPipelineConfiguration
{
    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    /// <param name="middleware"></param>
    ILoggerConfiguration Use(ILoggerMiddleware middleware);

    public ILoggerConfiguration Use<TMiddleware>() where TMiddleware : ILoggerMiddleware, new()
    {
        return Use(new TMiddleware());
    }

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration Use(Action<LoggerMessageContext> middleware)
    {
        return Use(new ActionMiddleware(middleware));
    }

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, ILoggerMiddleware middleware)
    {
        return Use(new ConditionalMiddleware(condition, middleware));
    }

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration UseWhen<TMiddleware>(Func<LoggerMessageContext, bool> condition)
        where TMiddleware : ILoggerMiddleware, new()
    {
        return UseWhen(condition, new TMiddleware());
    }

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, Action<LoggerMessageContext> middleware)
    {
        return UseWhen(condition, new ActionMiddleware(middleware));
    }
}
