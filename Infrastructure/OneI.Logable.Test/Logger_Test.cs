namespace OneI.Logable;

using OneI.Logable.Fakes;

public class Logger_Test
{
    [Fact]
    public void simple()
    {
        var log = new LoggerConfiguration()
            .Level.Error()
            .Level.Override(nameof(Microsoft), LogLevel.Information, LogLevel.Debug)
            .Sink.Use(new StringOutputWriter("{Timestamp:yyyy-MM-dd}{Message}"))
            .CreateLogger();

        log.Write(LogLevel.Debug, "message");
    }
}
