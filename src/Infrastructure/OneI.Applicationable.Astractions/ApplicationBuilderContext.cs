namespace OneI.Applicationable;

using Microsoft.Extensions.Configuration;
/// <summary>
/// The application builder context.
/// </summary>

public class ApplicationBuilderContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationBuilderContext"/> class.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <param name="configuration">The configuration.</param>
    public ApplicationBuilderContext(
        IApplicationEnvironment environment,
        IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    /// <summary>
    /// Gets the environment.
    /// </summary>
    public IApplicationEnvironment Environment { get; }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; }
}
