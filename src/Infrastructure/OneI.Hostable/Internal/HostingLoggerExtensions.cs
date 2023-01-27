namespace OneI.Hostable.Internal;

using System.Reflection;
using Microsoft.Extensions.Logging;
using static HostableConstants;

internal static class HostingLoggerExtensions
{
    public static void ApplicationError(this ILogger logger, EventId eventId, string? message, Exception? exception)
    {
        if(exception is ReflectionTypeLoadException reflectionTypeLoadException)
        {
            foreach(var ex in reflectionTypeLoadException.LoaderExceptions)
            {
                if(ex is not null)
                {
                    message = message + Environment.NewLine + ex.Message;
                }
            }
        }

        logger.LogCritical(
            eventId: eventId,
            exception: exception,
            message: message);
    }

    public static void HostStarting(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
               eventId: LoggerEventIds.Starting,
               message: "Hosting starting");
        }
    }

    public static void HostStarted(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: LoggerEventIds.Started,
                message: "Hosting started");
        }
    }

    public static void HostStopping(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: LoggerEventIds.Stopping,
                message: "Hosting stopping");
        }
    }

    public static void HostStopped(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: LoggerEventIds.Stopped,
                message: "Hosting stopped");
        }
    }

    public static void StoppedWithException(this ILogger logger, Exception? ex)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: LoggerEventIds.StoppedWithException,
                exception: ex,
                message: "Hosting shutdown exception");
        }
    }

    public static void BackgroundServiceFaulted(this ILogger logger, Exception? ex)
    {
        if(logger.IsEnabled(LogLevel.Error))
        {
            logger.LogError(
                eventId: LoggerEventIds.BackgroundServiceFaulted,
                exception: ex,
                message: "BackgroundService failed");
        }
    }

    public static void BackgroundServiceStoppingHost(this ILogger logger, Exception? ex)
    {
        if(logger.IsEnabled(LogLevel.Critical))
        {
            logger.LogCritical(
                eventId: LoggerEventIds.BackgroundServiceStoppingHost,
                exception: ex,
                message: $"A {nameof(BackgroundService)} has thrown an unhandled exception, and the {nameof(IHost)} instance is stopping.");
        }
    }
}
