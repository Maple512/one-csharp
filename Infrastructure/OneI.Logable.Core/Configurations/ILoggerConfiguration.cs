namespace OneI.Logable;

using OneI.Logable.Configurations;

public interface ILoggerConfiguration : ILoggerBranchConfiguration
{
    /// <summary>
    /// 表示<see cref="ILoggerLevelConfiguration"/>相关的配置
    /// </summary>
    ILoggerLevelConfiguration Level { get; }

    /// <summary>
    /// 表示<see cref="ILoggerPropertyConfiguration"/>相关的配置
    /// </summary>
    ILoggerPropertyConfiguration Properties { get; }

    /// <summary>
    /// 表示<see cref="ILoggerSink"/>相关的配置
    /// </summary>
    ILoggerSinkConfiguration Sink { get; }

    /// <summary>
    /// 表示
    /// </summary>
    ILoggerAuditConfiguration Audit { get; }

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
}
