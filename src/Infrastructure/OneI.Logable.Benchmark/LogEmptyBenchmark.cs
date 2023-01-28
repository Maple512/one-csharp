namespace OneI.Logable;

public class LogEmptyBenchmark : IValidator
{
    private const int count = 1;

    private static Serilog.Core.Logger? serilog;
    private static ILogger? logable;
    private static NLog.Logger? nlog;

    [GlobalSetup]
    public void Initialize()
    {
        serilog = new Serilog.LoggerConfiguration()
                .CreateLogger();

        logable = new LoggerConfiguration()
            .CreateLogger();

        nlog = NLog.LogManager.LoadConfiguration("nlog.empty.config").GetCurrentClassLogger();
    }

    [Benchmark(Baseline = true)]
    public void UseSeriLog()
    {
        for(var i = 0; i < count; i++)
        {
            serilog.Information(" {0} {1} {2} {3} ", 1, 2, 3, new object());
        }
    }

    [Benchmark]
    public void UseLogable()
    {
        for(var i = 0; i < count; i++)
        {
            logable.Information(" {0} {1} {2} {3} ", 1, 2, 3, new object());
        }
    }

    [Benchmark]
    public void UseNLog()
    {
        for(var i = 0; i < count; i++)
        {
            nlog.Info(" {0} {1} {2} {3} ", 1, 2, 3, new object());
        }
    }

    public void Validate()
    {
        Initialize();

        UseSeriLog();

        UseLogable();

        UseNLog();
    }
}
