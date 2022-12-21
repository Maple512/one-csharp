namespace OneI.Logable;

using OneI.Logable.Temporaries;

public static class LoggerTemporaryConfigurationExtensions
{
    /// <summary>
    /// 向管道中添加一个<see cref="TemporaryMiddleware"/>，用于对<see cref="LoggerContext"/>附加临时行为，释放后还原
    /// </summary>
    /// <remarks>
    ///添加配置：
    /// <code lang="C#">
    /// var logger = new LoggerConfiguration()
    ///                 .UseTemporaryContainer()
    ///                 .CreateLogger();
    /// </code>
    /// 使用方式:
    /// <code lang="C#">
    /// using (TemporaryContainer.PushProperty("name","value"))
    /// {
    ///     Log.Information("Now the attribute named "name" has been attached to LoggerContext");
    /// }
    /// </code>
    /// </remarks>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ILoggerConfiguration UseTemporaryContainer(this ILoggerConfiguration configuration)
    {
        configuration.Use(new TemporaryMiddleware());

        return configuration;
    }
}
