namespace OneI.Logable.Configurations;

public interface ILoggerBranchBuilder
{
    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    /// <param name="middleware"></param>
    /// <returns></returns>
    ILoggerBuilder Use(ILoggerMiddleware middleware);

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <returns></returns>
    ILoggerBuilder Use<TMiddleware>() where TMiddleware : ILoggerMiddleware, new();

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    /// <param name="middleware"></param>
    /// <returns></returns>
    ILoggerBuilder Use(Action<LoggerContext, LoggerDelegate> middleware);

    /// <summary>
    /// 向管道中添加一个中间件
    /// </summary>
    /// <param name="middleware"></param>
    /// <returns></returns>
    ILoggerBuilder Use(Action<LoggerContext> middleware);

    /// <summary>
    /// 向管道中添加一个带有条件的中间件
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    ILoggerBuilder UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> action);

    /// <summary>
    /// 当符合条件时，开启一个全新的分支
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="branch"></param>
    /// <returns></returns>
    ILoggerBuilder NewWhen(Func<LoggerContext, bool> condition, Action<ILoggerBuilder> branch);
}
