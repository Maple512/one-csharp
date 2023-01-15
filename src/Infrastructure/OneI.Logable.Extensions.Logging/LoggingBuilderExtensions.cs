namespace OneI.Logable;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddLogable(this ILoggingBuilder builder, ILogger logger)
    {
        var descriptor = ServiceDescriptor.Singleton<ILoggerProvider>(new LogableLoggerProvider(logger));

        builder.Services.TryAddEnumerable(descriptor);

        return builder;
    }

    public static ILoggingBuilder AddLogable(this ILoggingBuilder builder,Action<ILoggerConfiguration> configure)
    {
        var configuration = new LoggerConfiguration();

        configure(configuration);

        var logger = configuration.CreateLogger();

        var descriptor = ServiceDescriptor.Singleton<ILoggerProvider>(new LogableLoggerProvider(logger));

        builder.Services.TryAddEnumerable(descriptor);

        return builder;
    }
}
