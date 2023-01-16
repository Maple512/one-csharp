namespace OneI.Hostable;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;

public static class SystemdHostBuilderExtensions
{
    /// <summary>
    /// Configures the <see cref="IHost"/> lifetime to <see cref="SystemdLifetime"/>,
    /// provides notification messages for application started and stopping,
    /// and configures console logging to the systemd format.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This is context aware and will only activate if it detects the process is running
    ///     as a systemd Service.
    ///   </para>
    ///   <para>
    ///     The systemd service file must be configured with <c>Type=notify</c> to enable
    ///     notifications. See https://www.freedesktop.org/software/systemd/man/systemd.service.html.
    ///   </para>
    /// </remarks>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
    /// <returns>The <paramref name="hostBuilder"/> instance for chaining.</returns>
    public static IHostBuilder UseSystemd(this IHostBuilder hostBuilder)
    {
        if(SystemdHelper.IsSystemdService())
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                AddSystemdLifetime(services);
            });
        }
        return hostBuilder;
    }

    /// <summary>
    /// Configures the lifetime of the <see cref="IHost"/> built from <paramref name="services"/> to
    /// <see cref="SystemdLifetime"/>, provides notification messages for application started
    /// and stopping, and configures console logging to the systemd format.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This is context aware and will only activate if it detects the process is running
    ///     as a systemd Service.
    ///   </para>
    ///   <para>
    ///     The systemd service file must be configured with <c>Type=notify</c> to enable
    ///     notifications. See <see href="https://www.freedesktop.org/software/systemd/man/systemd.service.html"/>.
    ///   </para>
    /// </remarks>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> used to build the <see cref="IHost"/>.
    /// For example, <see cref="HostApplicationBuilder.Services"/> or the <see cref="IServiceCollection"/> passed to the
    /// <see cref="IHostBuilder.ConfigureServices(System.Action{HostBuilderContext, IServiceCollection})"/> callback.
    /// </param>
    /// <returns>The <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddSystemd(this IServiceCollection services)
    {
        if(SystemdHelper.IsSystemdService())
        {
            AddSystemdLifetime(services);
        }

        return services;
    }

    private static void AddSystemdLifetime(IServiceCollection services)
    {
        services.Configure<ConsoleLoggerOptions>(options =>
        {
            options.FormatterName = ConsoleFormatterNames.Systemd;
        });

        // IsSystemdService() will never return true for android/browser/iOS/tvOS
        services.AddSingleton<ISystemdNotifier, SystemdNotifier>();
        services.AddSingleton<IHostLifetime, SystemdLifetime>();
    }
}
