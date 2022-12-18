namespace OneI.Logable;

using OneI.Logable.Fakes;
using OneI.Logable.Middlewares;

public class Logger_Test
{
    [Fact]
    public void logger_test()
    {
        var logger = Fake.CreateLogger(
            configuration: config =>
            {
                config.Sink.Use(new TestAuditSink());
            },
            file: options =>
            {
                options.RenderWhen(c => c.Exception is not null, Fake.ErrorTemplate);
            });

        logger.ForContext<Middleware_Test>().Error(new ArgumentException(), "error {SourceContext} message");

        logger.Error(" de {SourceContext} bug ");

        logger.ForContext<IServiceProvider>().Error(new ArgumentException(), "error {SourceContext} message");

        logger.Error(" de {SourceContext} bug ");
    }
}
