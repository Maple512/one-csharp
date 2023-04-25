namespace OneI.Logable.Consoles;

public static class LoggerSinkConfigurationConsoleExtensions
{
    public static ILoggerConfiguration UseConsole(
        this ILoggerSinkConfiguration logger,
        Action<ConsoleOptions>? configure = null)
    {
        var options = new ConsoleOptions();

        configure?.Invoke(options);

        return logger.Use(new ConsoleSink(options));
    }

    public static ILoggerConfiguration UseConsoleWhen(
        this ILoggerSinkConfiguration logger,
        Func<LoggerContext, bool> condition,
        Action<ConsoleOptions>? configure = null)
    {
        var options = new ConsoleOptions();

        configure?.Invoke(options);

        return logger.UseWhen(condition, new ConsoleSink(options));
    }
}
