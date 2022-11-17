namespace OneI.Logable;
public interface ILoggerBuilder
{
    ILoggerBuilder Branch(LogDelegate branch);

    ILogger Build();
}
