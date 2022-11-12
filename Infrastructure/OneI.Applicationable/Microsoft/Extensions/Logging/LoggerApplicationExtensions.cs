namespace Microsoft.Extensions.Logging;

using System;
using System.Reflection;
using OneI.Applicationable;

public static class LoggerApplicationExtensions
{
    public static void ApplicationError(this ILogger logger, EventId eventId, string? message, Exception? exception)
    {
        if(exception is ReflectionTypeLoadException reflectionTypeLoadException)
        {
            foreach(var ex in reflectionTypeLoadException.LoaderExceptions)
            {
                if(ex is not null)
                {
                    message = $"{message}{Environment.NewLine}{ex.Message}";
                }
            }
        }

        logger.LogCritical(
            eventId: eventId,
            message: message,
            exception: exception);
    }

    public static void Starting(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
               eventId: ApplicationDefinition.LoggerEvents.Starting,
               message: "Application starting");
        }
    }

    public static void Started(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: ApplicationDefinition.LoggerEvents.Started,
                message: "Application started");
        }
    }

    public static void Stopping(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: ApplicationDefinition.LoggerEvents.Stopping,
                message: "Application stopping");
        }
    }

    public static void Stopped(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: ApplicationDefinition.LoggerEvents.Stopped,
                message: "Application stopped");
        }
    }

    public static void StoppedWithException(this ILogger logger, Exception? ex)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: ApplicationDefinition.LoggerEvents.StoppedWithException,
                exception: ex,
                message: "Application shutdown exception");
        }
    }

    public static void BackgroundServiceFaulted(this ILogger logger, Exception? ex)
    {
        if(logger.IsEnabled(LogLevel.Error))
        {
            logger.LogError(
                eventId: ApplicationDefinition.LoggerEvents.BackgroundServiceFaulted,
                exception: ex,
                message: "BackgroundService failed");
        }
    }

    public static void BackgroundServiceUnhandledException(this ILogger logger, Exception? ex)
    {
        if(logger.IsEnabled(LogLevel.Critical))
        {
            logger.LogCritical(
                eventId: ApplicationDefinition.LoggerEvents.BackgroundServiceStoppingHost,
                exception: ex,
                message: "A BackgroundService has thrown an unhandled exception, and the IApplication instance is stopping.");
        }
    }
}
