namespace OneI.Logable;

using Serilog;

public class LogFileBenchmark : IValidator
{
    private const int count = 1000;
    private static Serilog.Core.Logger? SeriLog;
    private static ILogger? logger;

    [GlobalSetup]
    public void Initialize()
    {
        SeriLog = new Serilog.LoggerConfiguration()
                .WriteTo.File("./Logs/serilog.log", rollingInterval: RollingInterval.Minute)
                .CreateLogger();

        logger = new LoggerConfiguration()
            .Sink.File("./Logs/logable.log", options =>
            {
                options.Frequency = RollFrequency.Minute;
            })
            .CreateLogger();
    }

    [Benchmark]
    public void UseSeriLog()
    {
        for(var i = 0; i < count; i++)
        {
            SeriLog.Information(" {0} {1} {2} {3} ", 1, 2, 3, new object());
        }
    }

    [Benchmark]
    public void UseLogable()
    {
        for(var i = 0; i < count; i++)
        {
            logger.Information(" {0} {1} {2} {3} ", 1, 2, 3, new object());
        }
    }

    public void Validate()
    {
        UseSeriLog();

        UseLogable();
    }
}
