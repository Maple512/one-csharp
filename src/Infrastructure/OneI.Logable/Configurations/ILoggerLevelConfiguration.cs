namespace OneI.Logable.Configurations;
/// <summary>
/// The logger level configuration.
/// </summary>

public interface ILoggerLevelConfiguration
{
    /// <summary>
    /// 设置日志等级
    /// </summary>
    /// <returns></returns>
    LoggerConfiguration Use(LogLevel minimum, LogLevel? maximum = null);

    /// <summary>
    /// 设置最小的日志等级
    /// </summary>
    /// <param name="minimum"></param>
    /// <returns></returns>
    ILoggerConfiguration Minimum(LogLevel minimum);

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Verbose"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Verbose();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Debug"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Debug();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Information"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Information();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Warning"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Warning();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Error"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Error();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Fatal"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Fatal();

    /// <summary>
    /// 设置最大的日志等级
    /// </summary>
    /// <param name="maximum"></param>
    /// <returns></returns>
    ILoggerConfiguration Maximum(LogLevel maximum);

    /// <summary>
    /// 重写来自指定命名空间或类型的日志等级
    /// </summary>
    /// <param name="sourceContext">指定的命名空间或类型</param>
    /// <param name="minimum">最小等级</param>
    /// <param name="maximum">最大等级</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    ILoggerConfiguration Override(string sourceContext, LogLevel minimum, LogLevel? maximum = null);
}
