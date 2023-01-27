namespace OneI.Logable.Configurations;

public interface ILoggerTemplateConfiguration
{
    /// <summary>
    /// 配置默认的日志模板
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    ILoggerConfiguration Default(string template);

    /// <summary>
    /// 配置具有条件的日志模板
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="template">模板</param>
    /// <returns></returns>
    ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, scoped ReadOnlySpan<char> template);
}
