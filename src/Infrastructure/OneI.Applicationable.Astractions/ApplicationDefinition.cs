namespace OneI.Applicationable;

using Microsoft.Extensions.Logging;
/// <summary>
/// The application definition.
/// </summary>

public static class ApplicationDefinition
{
    /// <summary>
    /// The configuration keys.
    /// </summary>
    public static class ConfigurationKeys
    {
        /// <summary>
        /// The environment name key.
        /// </summary>
        public const string EnvironmentNameKey = "environment";
        /// <summary>
        /// The application name key.
        /// </summary>
        public const string ApplicationNameKey = "application";
        /// <summary>
        /// The root path key.
        /// </summary>
        public const string RootPathKey = "root";
    }

    /// <summary>
    /// The shutdown timeout.
    /// </summary>
    public const int ShutdownTimeout = 30;

    /// <summary>
    /// The logger events.
    /// </summary>
    public static class LoggerEvents
    {
        public static readonly EventId Starting = new(1, nameof(Starting));
        public static readonly EventId Started = new(2, nameof(Started));
        public static readonly EventId Stopping = new(3, nameof(Stopping));
        public static readonly EventId Stopped = new(4, nameof(Stopped));
        public static readonly EventId StoppedWithException = new(5, nameof(StoppedWithException));
        public static readonly EventId ApplicationStartupException = new(6, nameof(ApplicationStartupException));
        public static readonly EventId ApplicationStoppingException = new(7, nameof(ApplicationStoppingException));
        public static readonly EventId ApplicationStoppedException = new(8, nameof(ApplicationStoppedException));
        public static readonly EventId BackgroundServiceFaulted = new(9, nameof(BackgroundServiceFaulted));
        public static readonly EventId BackgroundServiceStoppingHost = new(10, nameof(BackgroundServiceStoppingHost));
    }

    /// <summary>
    /// The environments.
    /// </summary>
    public static class Environments
    {
        /// <summary>
        /// The development.
        /// </summary>
        public const string Development = nameof(Development);
        /// <summary>
        /// The staging.
        /// </summary>
        public const string Staging = nameof(Staging);
        /// <summary>
        /// The production.
        /// </summary>
        public const string Production = nameof(Production);
    }
}
