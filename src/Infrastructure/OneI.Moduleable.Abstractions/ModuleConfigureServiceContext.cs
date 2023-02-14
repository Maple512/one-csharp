namespace OneI.Moduleable;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ModuleConfigureServiceContext
{
    public IServiceCollection Services { get; }

    public IConfiguration Configuration { get; }


}
