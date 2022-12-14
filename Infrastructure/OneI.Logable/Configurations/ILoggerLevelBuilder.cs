namespace OneI.Logable;

public interface ILoggerLevelBuilder
{
    /// <summary>
    /// 设置最小的日志等级
    /// </summary>
    /// <param name="minimum"></param>
    /// <returns></returns>
    ILoggerBuilder Minimum(LogLevel minimum);

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Verbose"/>
    /// </summary>
    /// <returns></returns>
    ILoggerBuilder Verbose();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Debug"/>
    /// </summary>
    /// <returns></returns>
    ILoggerBuilder Debug();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Information"/>
    /// </summary>
    /// <returns></returns>
    ILoggerBuilder Information();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Warning"/>
    /// </summary>
    /// <returns></returns>
    ILoggerBuilder Warning();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Error"/>
    /// </summary>
    /// <returns></returns>
    ILoggerBuilder Error();

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Fatal"/>
    /// </summary>
    /// <returns></returns>
    ILoggerBuilder Fatal();

    /// <summary>
    /// 设置最大的日志等级
    /// </summary>
    /// <param name="maximum"></param>
    /// <returns></returns>
    ILoggerBuilder Maximum(LogLevel maximum);

    /// <summary>
    /// 重写来自指定命名空间或类型的日志等级
    /// </summary>
    /// <param name="sourceContext">指定的命名空间或类型</param>
    /// <param name="minimum">最小等级</param>
    /// <param name="maximum">最大等级</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    ILoggerBuilder Override(string sourceContext, LogLevel minimum, LogLevel? maximum = null);
}
