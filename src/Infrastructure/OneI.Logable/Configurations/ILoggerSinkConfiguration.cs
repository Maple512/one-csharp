namespace OneI.Logable.Configurations;
/// <summary>
/// The logger sink configuration.
/// </summary>

public interface ILoggerSinkConfiguration
{
    /// <summary>
    /// Uses the.
    /// </summary>
    /// <param name="sink">The sink.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    ILoggerConfiguration Use(ILoggerSink sink);

    /// <summary>
    /// Uses the.
    /// </summary>
    /// <param name="sink">The sink.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    ILoggerConfiguration Use(Action<LoggerContext> sink);

    /// <summary>
    /// Uses the when.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="sink">The sink.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink sink);

    /// <summary>
    /// Uses the when.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="sink">The sink.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> sink);
}
