namespace OneI.Logable.Middlewares;

public class Middleware_Test
{
    [Fact]
    public void middleware_order()
    {
        var logger = LoggerTest.CreateLogger();

        logger.Write(LogLevel.Information, "Information message");

        logger.Write(LogLevel.Debug, " Debug message");

        logger.Write(LogLevel.Error, "error message");

        logger.Write(LogLevel.Error, new ArgumentException(), "error message");

        logger.ForContext<Middleware_Test>().Debug("{SourceContext}");
    }
}
