namespace OneI.Logable;

using Serilog;

public class LogFileBenchmark : BenchmarkItem
{
    private const int count = 1000;
    private static Serilog.Core.Logger? serilog;
    private static ILogger? logger;
    private static NLog.Logger? nlog;

    public override void GlobalInlitialize()
    {
        serilog = new Serilog.LoggerConfiguration()
                .WriteTo.File("./Logs/serilog.log", outputTemplate: "{NewLine}", rollingInterval: RollingInterval.Minute)
                .CreateLogger();

        logger = new LoggerConfiguration()
            .Template.Default("{NewLine}")
            .Sink.File("./Logs/logable.log", options =>
            {
                options.Frequency = RollFrequency.Minute;
            })
            .CreateLogger();

        nlog = NLog.LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();

        UseSeriLog();

        UseLogable();

        UseNLog();
    }

    [Benchmark(Baseline = true)]
    public void UseSeriLog()
    {
        for (var i = 0; i < count; i++)
        {
            serilog.Information(" {0} {1} {2} {3} ", 1, 2, 3, new object());
        }
    }

    [Benchmark]
    public void UseLogable()
    {
        for (var i = 0; i < count; i++)
        {
            logger.Information(" {0} {1} {2} {3} ", 1, 2, 3, new object());
        }
    }

    [Benchmark]
    public void UseNLog()
    {
        for (var i = 0; i < count; i++)
        {
            nlog.Info(" {0} {1} {2} {3} ", 1, 2, 3, new object());
        }
    }
}
