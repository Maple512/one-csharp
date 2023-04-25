namespace OneI.Logable;

public interface ILoggerPropertyConfiguration
{
    ILoggerConfiguration Add(string name, object value);

    ILoggerConfiguration AddOrUpdate(string name, object value);
}
