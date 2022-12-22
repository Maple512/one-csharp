namespace OneI.Logable.Infrastructure;
/// <summary>
/// The null logger.
/// </summary>

public class NullLogger : ILogger
{
    /// <summary>
    /// Gets the instance.
    /// </summary>
    public static ILogger Instance => new NullLogger();

    /// <summary>
    /// Are the enable.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <returns>A bool.</returns>
    public bool IsEnable(LogLevel level)
    {
        return false;
    }

    /// <summary>
    /// News the.
    /// </summary>
    /// <returns>An ILogger.</returns>
    public static ILogger New()
    {
        return Instance;
    }

    /// <summary>
    /// Writes the.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Write(LoggerContext context)
    {
    }

    /// <summary>
    /// Fors the context.
    /// </summary>
    /// <param name="middlewares">The middlewares.</param>
    /// <returns>An ILogger.</returns>
    public ILogger ForContext(params ILoggerMiddleware[] middlewares)
    {
        return Instance;
    }

    /// <summary>
    /// Fors the context.
    /// </summary>
    /// <param name="sourceContext">The source context.</param>
    /// <returns>An ILogger.</returns>
    public ILogger ForContext(string sourceContext)
    {
        return Instance;
    }

    /// <summary>
    /// Fors the context.
    /// </summary>
    /// <returns>An ILogger.</returns>
    public ILogger ForContext<TSourceContext>()
    {
        return Instance;
    }
}
