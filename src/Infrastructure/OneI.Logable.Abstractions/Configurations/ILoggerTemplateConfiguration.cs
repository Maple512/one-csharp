namespace OneI.Logable;

public interface ILoggerTemplateConfiguration
{
    ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, string template);
}
