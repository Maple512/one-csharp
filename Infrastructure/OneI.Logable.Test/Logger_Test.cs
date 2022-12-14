namespace OneI.Logable;

using OneI.Logable.Fakes;

public class Logger_Test
{
    [Fact]
    public void simple()
    {
        var log = new LoggerBuilder()
            .Level.Error()
            .Level.Override(nameof(Microsoft), LogLevel.Information, LogLevel.Debug)
            .Endpoint.Run(new StringOutputWriter("{Timestamp:yyyy-MM-dd}{Message}"))
            .CreateLogger();

        log.Write(LogLevel.Debug, "message");
    }
}
