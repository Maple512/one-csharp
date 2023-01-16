namespace OneI.Hostable;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class HostApplicationBuilderSettings
{
    public HostApplicationBuilderSettings()
    {
    }

    /// <summary>
    /// If <see langword="false"/>, configures the <see cref="HostApplicationBuilder"/> instance with pre-configured defaults.
    /// This has a similar effect to calling <see cref="HostingHostBuilderExtensions.ConfigureDefaults(IHostBuilder, string[])"/>.
    /// </summary>
    /// <remarks>
    ///   The following defaults are applied to the <see cref="IHostBuilder"/>:
    ///     * set the <see cref="IHostEnvironment.RootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/>
    ///     * load <see cref="IConfiguration"/> from "DOTNET_" prefixed environment variables
    ///     * load <see cref="IConfiguration"/> from 'appsettings.json' and 'appsettings.[<see cref="IHostEnvironment.EnvironmentName"/>].json'
    ///     * load <see cref="IConfiguration"/> from User Secrets when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development' using the entry assembly
    ///     * load <see cref="IConfiguration"/> from environment variables
    ///     * load <see cref="IConfiguration"/> from supplied command line args
    ///     * configure the <see cref="ILoggerFactory"/> to log to the console, debug, and event source output
    ///     * enables scope validation on the dependency injection container when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development'
    /// </remarks>
    public bool DisableDefaults { get; set; }

    /// <summary>
    /// The command line arguments. This is unused if <see cref="DisableDefaults"/> is <see langword="true"/>.
    /// </summary>
    public string[]? Args { get; set; }

    public ConfigurationManager? Configuration { get; set; }

    public string? EnvironmentName { get; set; }

    public string? ApplicationName { get; set; }

    public string? RootPath { get; set; }
}
