namespace OneI.Logable;

public class LoggerBuilder
{
    private readonly string _tempalte;

    public LoggerBuilder(string tempalte) => _tempalte = tempalte;

    public Logger Build() => new();
}
