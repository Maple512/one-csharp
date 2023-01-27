namespace OneI.Logable.Diagnostics;

public class EnvironmentMiddleware : ILoggerMiddleware
{
    private readonly EnvironmentOptions _options;

    public EnvironmentMiddleware(EnvironmentOptions options)
    {
        _options = options;
    }

    public void Invoke(LoggerMessageContext context)
    {
        if(_options.HasCommandLine)
        {
            context.AddProperty(nameof(Environment.CommandLine), Environment.CommandLine);
        }

        if(_options.HasCurrentDirectory)
        {
            context.AddProperty(nameof(Environment.CurrentDirectory), Environment.CurrentDirectory);
        }

        if(_options.HasCurrentManagedThreadId)
        {
            context.AddProperty(nameof(Environment.CurrentManagedThreadId), Environment.CurrentManagedThreadId);
        }

        if(_options.HasIs64BitOperatingSystem)
        {
            context.AddProperty(nameof(Environment.Is64BitOperatingSystem), Environment.Is64BitOperatingSystem);
        }

        if(_options.HasIs64BitProcess)
        {
            context.AddProperty(nameof(Environment.Is64BitProcess), Environment.Is64BitProcess);
        }

        if(_options.HasMachineName)
        {
            context.AddProperty(nameof(Environment.MachineName), Environment.MachineName);
        }

        if(_options.HasOSVersion)
        {
            context.AddProperty(nameof(Environment.OSVersion), Environment.OSVersion);
        }

        if(_options.HasProcessId)
        {
            context.AddProperty(nameof(Environment.ProcessId), Environment.ProcessId);
        }

        if(_options.HasProcessorCount)
        {
            context.AddProperty(nameof(Environment.ProcessorCount), Environment.ProcessorCount);
        }

        if(_options.HasProcessPath)
        {
            context.AddProperty(nameof(Environment.ProcessPath), Environment.ProcessPath);
        }

        if(_options.HasUserName)
        {
            context.AddProperty(nameof(Environment.UserName), Environment.UserName);
        }

        if(_options.HasFrameworkDescription)
        {
            context.AddProperty(nameof(RuntimeInformation.FrameworkDescription), RuntimeInformation.FrameworkDescription);
        }
    }
}

public static class EnvironmentMiddlewareExtensions
{
    public static ILoggerConfiguration WithEnvironment(
        this ILoggerConfiguration configuration,
        EnvironmentOptions options)
    {
        configuration.Use(new EnvironmentMiddleware(options));

        return configuration;
    }
}
