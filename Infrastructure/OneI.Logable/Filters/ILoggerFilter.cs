namespace OneI.Logable.Filters;

public interface ILoggerFilter
{
    bool IsEnabled(LoggerContext context);
}
