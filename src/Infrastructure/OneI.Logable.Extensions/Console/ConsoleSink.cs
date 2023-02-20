namespace OneI.Logable.Console;

using OneI.Logable;

internal sealed class ConsoleSink : ILoggerSink
{
    private ConsoleOptions _options;

    public ConsoleSink(ConsoleOptions options)
    {
        _options = options;
    }

    public void Invoke(in LoggerContext context)
    {
        lock(this)
        {
            var writer = context.Message.Exception is not null
                ? System.Console.Error
                : System.Console.Out;

            context.WriteTo(writer, _options.FormatProvider);
        }
    }
}
