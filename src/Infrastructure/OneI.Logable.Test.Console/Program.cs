namespace OneI.Logable;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        using var logger = new LoggerConfiguration()
            .Sink.FileWhen(c =>
            {
                return c.Context.Level == LogLevel.Verbose;
            }, $"./Logs/{nameof(LogLevel.Verbose)}log.log")
            .Sink.FileWhen(c =>
            {
                return c.Context.Level == LogLevel.Debug;
            }, $"./Logs/{nameof(LogLevel.Debug)}log.log")
            .Sink.FileWhen(c =>
            {
                return c.Context.Level == LogLevel.Information;
            }, $"./Logs/{nameof(LogLevel.Information)}log.log")
            .Sink.FileWhen(c =>
            {
                return c.Context.Level == LogLevel.Warning;
            }, $"./Logs/{nameof(LogLevel.Warning)}og.log")
            .Sink.FileWhen(c =>
            {
                return c.Context.Level == LogLevel.Error;
            }, $"./Logs/{nameof(LogLevel.Error)}og.log")
            .Sink.FileWhen(c =>
            {
                return c.Context.Level == LogLevel.Fatal;
            }, $"./Logs/{nameof(LogLevel.Fatal)}log.log")
            .CreateLogger();

        while(true)
        {
            logger.Verbose(" {0} {1} {2} {3} {4:HH:mm:ss}", 1, 2, 3, new object(), DateTime.Now);
            logger.Information(" {0} {1} {2} {3} {4:HH:mm:ss}", 1, 2, 3, new object(), DateTime.Now);
            logger.Debug(" {0} {1} {2} {3} {4:HH:mm:ss}", 1, 2, 3, new object(), DateTime.Now);
            logger.Warning(" {0} {1} {2} {3} {4:HH:mm:ss}", 1, 2, 3, new object(), DateTime.Now);
            logger.Error(" {0} {1} {2} {3} {4:HH:mm:ss}", 1, 2, 3, new object(), DateTime.Now);
            logger.Fatal(" {0} {1} {2} {3} {4:HH:mm:ss}", 1, 2, 3, new object(), DateTime.Now);

            await Task.Delay(TimeSpan.FromMilliseconds(1 * 100));
        }
    }
}
