namespace OneI.Logable.Configurations;

using Sinks;

public interface ILoggerSinkConfiguration
{
    ILoggerConfiguration Use(ILoggerSink sink);

    ILoggerConfiguration Use(Action<LoggerContext> sink) => Use(new ActionSink(sink));

    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink sink)
        => Use(new ConditionalSink(condition, sink));

    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> sink)
        => Use(new ConditionalSink(condition, new ActionSink(sink)));

    ILoggerConfiguration Logger(ILogger logger, bool autoDispose = false)
        => Use(new SecondaryLoggerSink(logger, autoDispose));

    ILoggerConfiguration LoggerWhen(Func<LoggerContext, bool> condition, ILogger logger, bool autoDispose = false)
        => UseWhen(condition, new SecondaryLoggerSink(logger, autoDispose));

    ILoggerConfiguration Logger(Action<ILoggerConfiguration> configure, bool autoDispose = false)
    {
        var configuration = new LoggerConfiguration();
        configure(configuration);
        var logger = configuration.CreateLogger();

        return Logger(logger, autoDispose);
    }

    ILoggerConfiguration LoggerWhen(Func<LoggerContext, bool> condition, Action<ILoggerConfiguration> configure, bool autoDispose = false)
    {
        var configuration = new LoggerConfiguration();
        configure(configuration);
        var logger = configuration.CreateLogger();

        return LoggerWhen(condition, logger, autoDispose);
    }
}
