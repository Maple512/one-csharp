namespace OneI.Logable;

using OneI.Logable.Templates;

internal class EnvironmentMiddleware : ILoggerMiddleware
{
    private readonly EnvironmentOptions _options;

    public EnvironmentMiddleware(EnvironmentOptions options) => _options = options;

    public void Invoke(in LoggerMessageContext context, ref PropertyDictionary properties)
    {
        if(_options.HasCommandLine)
        {
            properties.Add(nameof(Environment.CommandLine), Environment.CommandLine);
        }

        if(_options.HasCurrentDirectory)
        {
            properties.Add(nameof(Environment.CurrentDirectory), Environment.CurrentDirectory);
        }

        if(_options.HasCurrentManagedThreadId)
        {
            properties.Add(nameof(Environment.CurrentManagedThreadId), Environment.CurrentManagedThreadId);
        }

        if(_options.HasIs64BitOperatingSystem)
        {
            properties.Add(nameof(Environment.Is64BitOperatingSystem), Environment.Is64BitOperatingSystem);
        }

        if(_options.HasIs64BitProcess)
        {
            properties.Add(nameof(Environment.Is64BitProcess), Environment.Is64BitProcess);
        }

        if(_options.HasMachineName)
        {
            properties.Add(nameof(Environment.MachineName), Environment.MachineName);
        }

        if(_options.HasOSVersion)
        {
            properties.Add(nameof(Environment.OSVersion), Environment.OSVersion);
        }

        if(_options.HasProcessId)
        {
            properties.Add(nameof(Environment.ProcessId), Environment.ProcessId);
        }

        if(_options.HasProcessorCount)
        {
            properties.Add(nameof(Environment.ProcessorCount), Environment.ProcessorCount);
        }

        if(_options.HasProcessPath)
        {
            properties.Add(nameof(Environment.ProcessPath), Environment.ProcessPath);
        }

        if(_options.HasUserName)
        {
            properties.Add(nameof(Environment.UserName), Environment.UserName);
        }

        if(_options.HasFrameworkDescription)
        {
            properties.Add(nameof(RuntimeInformation.FrameworkDescription), RuntimeInformation.FrameworkDescription);
        }
    }
}
