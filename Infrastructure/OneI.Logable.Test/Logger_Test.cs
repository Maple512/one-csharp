namespace OneI.Logable;

using OneI.Logable.Fakes;
using OneI.Logable.Middlewares;

public class Logger_Test
{
    [Fact]
    public void logger_test()
    {
        var logger = Fake.CreateLogger();

        logger.Write(LogLevel.Information, "Information message");

        logger.Write(LogLevel.Debug, "Debug message");

        logger.Write(LogLevel.Error, "error message");

        logger.Write(LogLevel.Error, new ArgumentException(), "error message");

        logger.ForContext<Middleware_Test>().Debug("{SourceContext}");
    }
}
