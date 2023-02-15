namespace OneI.Logable;

using Microsoft.Extensions.Logging;
using NLog;
using Serilog;
using ZLogger;

public class WriteFileBenchmark : BenchmarkItem
{
    private static Serilog.Core.Logger serilog;
    private static ILogger logger;
    private static NLog.Logger nlog;
    private static Microsoft.Extensions.Logging.ILogger zlogger;

    private const int count = 1000;

    public override void Inlitialize()
    {
        serilog = new Serilog.LoggerConfiguration()
                  .WriteTo.File("./Logs/serilog.log",
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss}[{Level}]{Message}{NewLine}",
                                rollingInterval: RollingInterval.Hour)
                  .CreateLogger();

        logger = new LoggerConfiguration("{Timestamp:yyyy-MM-dd HH:mm:ss}[{Level}]{Message}{NewLine}")
                 .Sink.RollFile("./Logs/logable.log")
                 .CreateLogger();

        nlog = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();

        zlogger = LoggerFactory.Create(builder =>
        {
            _ = builder.AddZLoggerRollingFile((time, sequence) => $"./Logs/zlog{time:yyyyHHmmdd}_{sequence}.log",
                                          time => new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, 0, 0
                                                                     , TimeSpan.Zero),
                                          1 * 1024 * 1024);
        }).CreateLogger(nameof(WriteFileBenchmark));
    }

    [Benchmark(Baseline = true)]
    public void UseSeriLog()
    {
        for(var i = 0; i < count; i++)
        {
            serilog.Information(" {0} {1} {2} {3}", 0, "", new object());
        }
    }

    [Benchmark]
    public void UseLogable()
    {
        for(var i = 0; i < count; i++)
        {
            logger.Information(" {0} {1} {2} {3}", 0, "", new object());
        }
    }

    [Benchmark]
    public void UseNLog()
    {
        for(var i = 0; i < count; i++)
        {
            nlog.Info(" {0} {1} {2} {3}", 0, "", new object());
        }
    }

    [Benchmark]
    public void UseZLog()
    {
        for(var i = 0; i < count; i++)
        {
            zlogger.ZLogInformation(" {0} {1} {2} {3}", 0, "", new object());
        }
    }
}
