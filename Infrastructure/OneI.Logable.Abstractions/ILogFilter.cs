namespace OneI.Logable;

public interface ILogFilter
{
    bool Filter(ILoggerContext context);
}
