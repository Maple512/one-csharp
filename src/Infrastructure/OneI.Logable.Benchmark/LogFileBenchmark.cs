namespace OneI.Logable;

using AutoBogus;
using BenchmarkDotNet.Engines;
using Microsoft.Extensions.Logging;
using Moq;
using NLog;
using Serilog;
using ZLogger;
using OneI.Logable.CodeGenerated;

[SimpleJob(RunStrategy.Throughput, baseline: true)]
public class LogFileBenchmark : BenchmarkItem
{
    private static Serilog.Core.Logger serilog;
    private static ILogger logger;
    private static NLog.Logger nlog;
    private static Microsoft.Extensions.Logging.ILogger zlogger;

    private const int count = 1000;

    static List<Model1> _models;

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
        }).CreateLogger(nameof(LogFileBenchmark));

        _models = AutoFaker.Create().Generate<List<Model1>>();
    }

    [Benchmark(Baseline = true)]
    public void UseSeriLog()
    {
        for(var i = 0; i < count; i++)
        {
            serilog.Information(" {0} ", _models);
        }
    }

    [Benchmark]
    public void UseLogable()
    {
        for(var i = 0; i < count; i++)
        {
            logger.Information(" {0} ", _models);
        }
    }

    [Benchmark]
    public void UseNLog()
    {
        for(var i = 0; i < count; i++)
        {
            nlog.Info(" {0} ", _models);
        }
    }

    //[Benchmark]
    public void UseZLog()
    {
        for(var i = 0; i < count; i++)
        {
            zlogger.ZLogInformation(" {0}  ", _models);
        }
    }

    public class Model1
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public int Age { get; set; }

        public Dictionary<int, string> Profiles { get; set; }
    }
}
