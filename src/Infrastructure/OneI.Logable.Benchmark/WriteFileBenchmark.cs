namespace OneI.Logable;

using DotNext.Reflection;
using NLog;
using Serilog;

using MSLogging = Microsoft.Extensions.Logging;

public partial class WriteFileBenchmark : BenchmarkItem
{
    private static Serilog.Core.Logger serilog;
    private static ILogger logger;
    private static NLog.Logger nlog;

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

    [MSLogging.LoggerMessage(14, MSLogging.LogLevel.Debug, @"Connection id ""{ConnectionId}"" communication error.", EventName = "ConnectionError", SkipEnabledCheck = true)]
    private static partial void ConnectionErrorCore(MSLogging.ILogger logger, string connectionId, Exception ex);

    public static void ConnectionError(MSLogging.ILogger logger, Exception ex)
    {
        if(logger.IsEnabled(MSLogging.LogLevel.Debug))
        {
            ConnectionErrorCore(logger, Guid.NewGuid().ToString("N"), ex);
        }
    }
}
