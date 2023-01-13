namespace OneI.Logable.Configurations;

using OneI.Logable;

public interface ILoggerLevelConfiguration
{
    /// <summary>
    /// 设置日志等级
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Use(LogLevel minimum, LogLevel? maximum = null);

    /// <summary>
    /// 设置最小的日志等级
    /// </summary>
    /// <param name="minimum"></param>
    /// <returns></returns>
    ILoggerConfiguration Minimum(LogLevel minimum) => Use(minimum, null);

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Verbose"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Verbose() => Use(LogLevel.Verbose, null);

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Debug"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Debug() => Use(LogLevel.Debug, null);

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Information"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Information() => Use(LogLevel.Information, null);

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Warning"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Warning() => Use(LogLevel.Warning, null);

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Error"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Error() => Use(LogLevel.Error, null);

    /// <summary>
    /// 设置日志的最小等级为<see cref="LogLevel.Fatal"/>
    /// </summary>
    /// <returns></returns>
    ILoggerConfiguration Fatal() => Use(LogLevel.Fatal, null);

    /// <summary>
    /// 重写来自指定命名空间或类型的日志等级
    /// </summary>
    /// <param name="sourceContext">指定的命名空间或类型</param>
    /// <param name="minimum">最小等级</param>
    /// <param name="maximum">最大等级</param>
    /// <returns></returns>
    ILoggerConfiguration Override(string sourceContext, LogLevel minimum, LogLevel? maximum = null);

    /// <summary>
    /// 重写来自指定命名空间或类型的日志等级
    /// </summary>
    /// <typeparam name="T">指定的类型</typeparam>
    /// <param name="minimum">最小等级</param>
    /// <param name="maximum">最大等级</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    ILoggerConfiguration Override<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(LogLevel minimum, LogLevel? maximum = null)
        => Override(typeof(T).FullName!, minimum, maximum);
}
