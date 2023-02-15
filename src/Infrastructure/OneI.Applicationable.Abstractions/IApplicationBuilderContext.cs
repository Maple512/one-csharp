namespace OneI.Applicationable;

using Microsoft.Extensions.Configuration;

public interface IApplicationBuilderContext
{
    IApplicationEnvironment Environment { get; }

    IConfiguration Configuration { get; }
}
