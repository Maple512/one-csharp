namespace OneI.Logable;

using OneI.Logable.Configurations;

public interface ILoggerBuilder : ILoggerBranchBuilder
{
    /// <summary>
    /// 表示<see cref="ILoggerLevelBuilder"/>相关的配置
    /// </summary>
    ILoggerLevelBuilder Level { get; }

    /// <summary>
    /// 表示<see cref="ILoggerPropertyBuilder"/>相关的配置
    /// </summary>
    ILoggerPropertyBuilder Properties { get; }

    /// <summary>
    /// 表示<see cref="ILoggerEndpoint"/>相关的配置
    /// </summary>
    ILoggerEndpointBuilder Endpoint { get; }

    /// <summary>
    /// 组装中间件
    /// </summary>
    /// <returns></returns>
    LoggerDelegate PackageMiddleware();

    /// <summary>
    /// 创建一个<see cref="ILogger"/>
    /// </summary>
    /// <returns></returns>
    ILogger CreateLogger();

    /// <summary>
    /// 创建一个新的<see cref="ILoggerBuilder"/>
    /// </summary>
    /// <returns></returns>
    ILoggerBuilder New();
}
