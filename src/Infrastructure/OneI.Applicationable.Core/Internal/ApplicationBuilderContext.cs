namespace OneI.Applicationable.Internal;

using Microsoft.Extensions.Configuration;

internal sealed class ApplicationBuilderContext : IApplicationBuilderContext
{
    public ApplicationBuilderContext(IApplicationEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    public IApplicationEnvironment Environment { get; }

    public IConfiguration Configuration { get; internal set; }
}
