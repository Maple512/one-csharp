namespace Microsoft.Extensions.Logging;

using System;
using System.Reflection;
using OneI.Applicationable;
/// <summary>
/// The logger application extensions.
/// </summary>

public static class LoggerApplicationExtensions
{
    /// <summary>
    /// Applications the error.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="eventId">The event id.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
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

    /// <summary>
    /// Startings the.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public static void Starting(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
               eventId: ApplicationDefinition.LoggerEvents.Starting,
               message: "Application starting");
        }
    }

    /// <summary>
    /// Starteds the.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public static void Started(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: ApplicationDefinition.LoggerEvents.Started,
                message: "Application started");
        }
    }

    /// <summary>
    /// Stoppings the.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public static void Stopping(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: ApplicationDefinition.LoggerEvents.Stopping,
                message: "Application stopping");
        }
    }

    /// <summary>
    /// Stoppeds the.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public static void Stopped(this ILogger logger)
    {
        if(logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                eventId: ApplicationDefinition.LoggerEvents.Stopped,
                message: "Application stopped");
        }
    }

    /// <summary>
    /// Stoppeds the with exception.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="ex">The ex.</param>
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

    /// <summary>
    /// Backgrounds the service faulted.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="ex">The ex.</param>
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

    /// <summary>
    /// Backgrounds the service unhandled exception.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="ex">The ex.</param>
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
