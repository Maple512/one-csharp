namespace OneI.Logable.Diagnostics;

using System.Runtime.InteropServices;

public class EnvironmentMiddleware : ILoggerMiddleware
{
    private readonly EnvironmentOptions _options;

    public EnvironmentMiddleware(EnvironmentOptions options)
    {
        _options = options;
    }

    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        if(_options.HasCommandLine)
        {
            context.AddPropertyIfAbsent(nameof(Environment.CommandLine), Environment.CommandLine);
        }

        if(_options.HasCurrentDirectory)
        {
            context.AddPropertyIfAbsent(nameof(Environment.CurrentDirectory), Environment.CurrentDirectory);
        }

        if(_options.HasCurrentManagedThreadId)
        {
            context.AddPropertyIfAbsent(nameof(Environment.CurrentManagedThreadId), Environment.CurrentManagedThreadId);
        }

        if(_options.HasIs64BitOperatingSystem)
        {
            context.AddPropertyIfAbsent(nameof(Environment.Is64BitOperatingSystem), Environment.Is64BitOperatingSystem);
        }

        if(_options.HasIs64BitProcess)
        {
            context.AddPropertyIfAbsent(nameof(Environment.Is64BitProcess), Environment.Is64BitProcess);
        }

        if(_options.HasMachineName)
        {
            context.AddPropertyIfAbsent(nameof(Environment.MachineName), Environment.MachineName);
        }

        if(_options.HasOSVersion)
        {
            context.AddPropertyIfAbsent(nameof(Environment.OSVersion), Environment.OSVersion);
        }

        if(_options.HasProcessId)
        {
            context.AddPropertyIfAbsent(nameof(Environment.ProcessId), Environment.ProcessId);
        }

        if(_options.HasProcessorCount)
        {
            context.AddPropertyIfAbsent(nameof(Environment.ProcessorCount), Environment.ProcessorCount);
        }

        if(_options.HasProcessPath)
        {
            context.AddPropertyIfAbsent(nameof(Environment.ProcessPath), Environment.ProcessPath);
        }

        if(_options.HasUserName)
        {
            context.AddPropertyIfAbsent(nameof(Environment.UserName), Environment.UserName);
        }

        if(_options.HasFrameworkDescription)
        {
            context.AddPropertyIfAbsent(nameof(RuntimeInformation.FrameworkDescription), RuntimeInformation.FrameworkDescription);
        }

        return next(context);
    }
}

public static class EnvironmentMiddlewareExtensions
{
    /// <summary>
    /// 在管道中附加进程ID
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ILoggerConfiguration WithEnvironment(
        this ILoggerConfiguration configuration,
        EnvironmentOptions options)
    {
        configuration.Use(new EnvironmentMiddleware(options));

        return configuration;
    }
}
