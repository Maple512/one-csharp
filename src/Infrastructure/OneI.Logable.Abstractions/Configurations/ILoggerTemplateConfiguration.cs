namespace OneI.Logable;

public interface ILoggerTemplateConfiguration
{
    ILoggerConfiguration Default(string template);

    ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, string template);
}
