namespace OneI.Applicationable;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneI.Moduleable;

public class Program : AppProgram
{
    public static async Task Main(string[] args)
    {
        var logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole().AddDebug();
        }).CreateLogger("Program");

        var configurationBuilderAction = ConfigureConfiguration;

        await AppProgram.RunAsync<Program>(configurationBuilderAction, logger);
    }

    public static void ConfigureConfiguration(IConfigurationBuilder builder)
    {
        builder.AddJsonFile("appsettings.json");
    }

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

    public override ValueTask ConfigureAsync(ModuleConfigureContext context)
    {
        return base.ConfigureAsync(context);
    }
}
