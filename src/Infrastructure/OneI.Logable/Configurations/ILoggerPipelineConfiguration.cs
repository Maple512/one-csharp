namespace OneI.Logable.Configurations;

using OneI.Logable.Middlewares;

public interface ILoggerPipelineConfiguration
{
    /// <summary>
    ///     向管道中添加一个中间件
    /// </summary>
    /// <param name="middleware"></param>
    ILoggerConfiguration With(ILoggerMiddleware middleware);

    public ILoggerConfiguration With<TMiddleware>()
        where TMiddleware : ILoggerMiddleware, new()
        => With(new TMiddleware());

    /// <summary>
    ///     向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration With(Action<LoggerMessageContext> middleware) => With(new ActionMiddleware(middleware));

    /// <summary>
    ///     向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration WithWhen(Func<LoggerMessageContext, bool> condition, ILoggerMiddleware middleware)
        => With(new ConditionalMiddleware(condition, middleware));

    /// <summary>
    ///     向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration WithWhen<TMiddleware>(Func<LoggerMessageContext, bool> condition)
        where TMiddleware : ILoggerMiddleware, new()
        => WithWhen(condition, new TMiddleware());

    /// <summary>
    ///     向管道中添加一个中间件
    /// </summary>
    public ILoggerConfiguration WithWhen(Func<LoggerMessageContext, bool> condition
                                        , Action<LoggerMessageContext> middleware)
        => WithWhen(condition, new ActionMiddleware(middleware));
}
