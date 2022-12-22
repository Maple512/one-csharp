namespace OneI.Logable;
/// <summary>
/// The logger.
/// </summary>

public interface ILogger
{
    /// <summary>
    /// Are the enable.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <returns>A bool.</returns>
    bool IsEnable(LogLevel level);

    /// <summary>
    /// Writes the.
    /// </summary>
    /// <param name="context">The context.</param>
    void Write(LoggerContext context);

    /// <summary>
    /// Fors the context.
    /// </summary>
    /// <param name="middlewares">The middlewares.</param>
    /// <returns>An ILogger.</returns>
    ILogger ForContext(params ILoggerMiddleware[] middlewares);

    /// <summary>
    /// Fors the context.
    /// </summary>
    /// <param name="sourceContext">The source context.</param>
    /// <returns>An ILogger.</returns>
    ILogger ForContext(string sourceContext);

    /// <summary>
    /// Fors the context.
    /// </summary>
    /// <returns>An ILogger.</returns>
    ILogger ForContext<TSourceContext>();
}
