namespace OneI.Logable.Configurations;

using OneI.Logable;

public interface ILoggerTemplateConfiguration
{
    /// <summary>
    /// 配置具有条件的日志模板
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="template">模板</param>
    /// <returns></returns>
    ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, string template);
}
