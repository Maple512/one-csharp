namespace OneI.Applicationable;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneI.Moduleable;
/// <summary>
/// The program.
/// </summary>

public class Program : AppProgram
{
    /// <summary>
    /// Mains the.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>A Task.</returns>
    public static async Task Main(string[] args)
    {
        var logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole().AddDebug();
        }).CreateLogger("Program");

        var configurationBuilderAction = ConfigureConfiguration;

        await AppProgram.RunAsync<Program>(configurationBuilderAction, logger);
    }

    /// <summary>
    /// Configures the configuration.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void ConfigureConfiguration(IConfigurationBuilder builder)
    {
        builder.AddJsonFile("appsettings.json");
    }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void ConfigureServices(in ModuleConfigureServiceContext context)
    {
        context.Services.Configure<ApplicationOptions>(context.Configuration);

        context.Services.AddLogging(builder =>
        {
            builder.AddConsole().AddDebug();
        });

        if(context.Environment.IsDevelopment())
        {
        }
    }

    /// <summary>
    /// Configures the async.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A ValueTask.</returns>
    public override ValueTask ConfigureAsync(ModuleConfigureContext context)
    {
        return base.ConfigureAsync(context);
    }
}
