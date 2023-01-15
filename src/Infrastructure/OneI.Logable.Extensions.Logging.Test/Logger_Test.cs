namespace OneI.Logable;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Logger_Test
{
    [Fact]
    public void create_logger()
    {
        var messages = new List<LoggerMessageContext>(20);
        Log.Initialize(configure =>
        {
            configure.Sink.Use(x =>
            {
                messages.Add(x.MessageContext);
            });
        });

        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddConsole();
            builder.AddLogable(Log.GetLogger());
        });

        var provider = services.BuildServiceProvider();

        var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Logger_Test>>();

        logger.LogInformation("Information message");

        using(logger.BeginScope(100))
        {
            logger.LogInformation("The Scope is: {Scope}.", 1000);
        }
    }
}
