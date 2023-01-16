namespace OneI.Hostable;

using Microsoft.Extensions.Configuration;

public class HostBuilderContext
{
    public HostBuilderContext(IHostEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    public IHostEnvironment Environment { get; set; }

    public IConfiguration Configuration { get; set; }
}
