namespace OneI.Logable;

using OneI.Logable.Fakes;

public class Logger_Test
{
    [Fact]
    public void simple()
    {
        var log = new LoggerBuilder()
            .WriteTo(new StringOutputWriter("{Timestamp:yyyy-MM-dd}{Message}"))
            .Build();

        log.Write(LogLevel.Debug, "message");
    }
}
