namespace OneI.Logable;

public static class EnvironmentMiddlewareExtensions
{
    public static ILoggerConfiguration WithEnvironment(
        this ILoggerConfiguration logger,
        Action<EnvironmentOptions> configure)
    {
        var options = new EnvironmentOptions();

        configure.Invoke(options);

        return logger.With(new EnvironmentMiddleware(options));
    }

    public static ILoggerConfiguration WithEnvironmentWhen(
        this ILoggerConfiguration logger,
        Func<LoggerMessageContext, bool> condition,
        Action<EnvironmentOptions> configure)
    {
        var options = new EnvironmentOptions();

        configure.Invoke(options);

        return logger.WithWhen(condition, new EnvironmentMiddleware(options));
    }
}
