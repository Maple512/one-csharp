namespace OneI.Logable;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        using var logger = new LoggerConfiguration()
            .Sink.File("./Logs/{Level}/log.log", options =>
            {
                options.Frequency = RollFrequency.Minute;
                options.CountLimit = 2;
            })
            .CreateLogger();

        while(true)
        {
            logger.Information(" {0} {1} {2} {3} {4:HH:mm:ss}", 1, 2, 3, new object(), DateTime.Now);

            await Task.Delay(TimeSpan.FromSeconds(20));
        }
    }
}
