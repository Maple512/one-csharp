namespace OneI.Logable;

using OneT.Common;

public class Logger_Test
{
    [Fact]
    public void create_logger_from_configuration()
    {
        var path = TestTools.GetFilePathWithinProject("./Logs/log.txt");

        var logger = new LoggerConfiguration()
            .WriteTo.File(path, template: "[{@User}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .Enrich.WithProperty("User", TestHelpler.CreateNewUser())
            .CreateLogger();

        logger.Write(LogLevel.Debug, "test message");
    }
}
