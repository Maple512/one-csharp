namespace OneI.Applicationable;

using Microsoft.Extensions.Configuration;

public class ApplicationBuilderContext
{
    public ApplicationBuilderContext(
        IApplicationEnvironment environment,
        IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    public IApplicationEnvironment Environment { get; }

    public IConfiguration Configuration { get; }
}
