namespace OneI.Logable.Configurations;

using System;
using OneI.Logable.Sinks;

public static class LoggerSinkConfigurationExtensions
{
    /// <summary>
    /// 启用一个<see cref="ILogger"/>副本
    /// </summary>
    /// <param name="sink"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ILoggerConfiguration UseSecondary(
        this ILoggerSinkConfiguration sink,
        ILogger logger)
    {
        return sink.Use(new SecondaryLoggerSink(logger));
    }

    /// <summary>
    /// 启用一个<see cref="ILogger"/>副本
    /// </summary>
    /// <param name="sink"></param>
    /// <param name="loggerConfiguration"></param>
    /// <returns></returns>
    public static ILoggerConfiguration UseSecondary(
        this ILoggerSinkConfiguration sink,
        Action<ILoggerConfiguration> loggerConfiguration)
    {
        var configuration = new LoggerConfiguration();

        loggerConfiguration(configuration);

        var logger = configuration.CreateLogger();

        return sink.Use(new SecondaryLoggerSink(logger));
    }

    /// <summary>
    /// 启用一个<see cref="ILogger"/>副本
    /// </summary>
    /// <param name="sink"></param>
    /// <param name="condition"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ILoggerConfiguration UseSecondaryWhen(
        this ILoggerSinkConfiguration sink,
        Func<LoggerContext, bool> condition,
        ILogger logger)
    {
        return sink.UseWhen(condition, new SecondaryLoggerSink(logger));
    }

    /// <summary>
    /// 启用一个<see cref="ILogger"/>副本
    /// </summary>
    /// <param name="sink"></param>
    /// <param name="condition"></param>
    /// <param name="loggerConfiguration"></param>
    /// <returns></returns>
    public static ILoggerConfiguration UseSecondaryWhen(
        this ILoggerSinkConfiguration sink,
        Func<LoggerContext, bool> condition,
        Action<ILoggerConfiguration> loggerConfiguration)
    {
        var configuration = new LoggerConfiguration();
        loggerConfiguration(configuration);
        var logger = configuration.CreateLogger();

        return sink.UseWhen(condition, new SecondaryLoggerSink(logger));
    }
}
