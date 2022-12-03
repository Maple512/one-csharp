namespace OneI.Logable;

public class Logger_Test
{
    [Fact]
    public void write_message()
    {
        var logger = new LoggerBuilder("{Message}")

            .Build();

        logger.Write(LogLevel.Debug, "");
    }
}
