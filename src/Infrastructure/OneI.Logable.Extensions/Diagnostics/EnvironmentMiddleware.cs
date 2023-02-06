namespace OneI.Logable.Diagnostics;

using OneI.Logable.Templates;

public class EnvironmentMiddleware : ILoggerMiddleware
{
    private readonly EnvironmentOptions _options;

    public EnvironmentMiddleware(EnvironmentOptions options) => _options = options;

    public void Invoke(in LoggerMessageContext context, ref PropertyDictionary properties)
    {
        if(_options.HasCommandLine)
        {
            properties.AddProperty(nameof(Environment.CommandLine), Environment.CommandLine);
        }

        if(_options.HasCurrentDirectory)
        {
            properties.AddProperty(nameof(Environment.CurrentDirectory), Environment.CurrentDirectory);
        }

        if(_options.HasCurrentManagedThreadId)
        {
            properties.AddProperty(nameof(Environment.CurrentManagedThreadId), Environment.CurrentManagedThreadId);
        }

        if(_options.HasIs64BitOperatingSystem)
        {
            properties.AddProperty(nameof(Environment.Is64BitOperatingSystem), Environment.Is64BitOperatingSystem);
        }

        if(_options.HasIs64BitProcess)
        {
            properties.AddProperty(nameof(Environment.Is64BitProcess), Environment.Is64BitProcess);
        }

        if(_options.HasMachineName)
        {
            properties.AddProperty(nameof(Environment.MachineName), Environment.MachineName);
        }

        if(_options.HasOSVersion)
        {
            properties.AddProperty(nameof(Environment.OSVersion), Environment.OSVersion);
        }

        if(_options.HasProcessId)
        {
            properties.AddProperty(nameof(Environment.ProcessId), Environment.ProcessId);
        }

        if(_options.HasProcessorCount)
        {
            properties.AddProperty(nameof(Environment.ProcessorCount), Environment.ProcessorCount);
        }

        if(_options.HasProcessPath)
        {
            properties.AddProperty(nameof(Environment.ProcessPath), Environment.ProcessPath);
        }

        if(_options.HasUserName)
        {
            properties.AddProperty(nameof(Environment.UserName), Environment.UserName);
        }

        if(_options.HasFrameworkDescription)
        {
            properties.AddProperty(nameof(RuntimeInformation.FrameworkDescription), RuntimeInformation.FrameworkDescription);
        }
    }
}

public static class EnvironmentMiddlewareExtensions
{
    public static ILoggerConfiguration WithEnvironment(
        this ILoggerConfiguration configuration,
        EnvironmentOptions options)
    {
        _ = configuration.Use(new EnvironmentMiddleware(options));

        return configuration;
    }
}
