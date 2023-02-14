namespace OneI.Logable;

public interface ILoggerPropertyConfiguration
{
    ILoggerConfiguration Add<T>(string name, T value);

    ILoggerConfiguration AddOrUpdate<T>(string name, T value);
}
