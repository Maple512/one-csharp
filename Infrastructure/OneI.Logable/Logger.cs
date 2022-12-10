namespace OneI.Logable;

using OneI.Logable.Middlewares;

public class Logger : ILogger
{
    private readonly LoggerDelegate _process;
    private readonly IReadOnlyList<LoggerDelegate> _writers;

    internal Logger(LoggerDelegate process, IReadOnlyList<LoggerDelegate> writers)
    {
        _process = process;
        _writers = writers;
    }

    public bool IsEnable(LogLevel level)
    {
        throw new NotImplementedException();
    }

    public void Write(LoggerContext context)
    {
        if(context is not null
            && IsEnable(context.Level))
        {
            Dispatch(context);
        }
    }

    private void Dispatch(LoggerContext context)
    {
        try
        {
            _process.Invoke(context);
        }
        catch(Exception)
        {

        }

        foreach(var writer in _writers)
        {
            try
            {
                writer(context);
            }
            catch(Exception) { }
        }
    }
}
