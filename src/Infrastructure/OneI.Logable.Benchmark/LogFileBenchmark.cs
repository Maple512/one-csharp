namespace OneI.Logable;

using Serilog;

public class LogFileBenchmark : IValidator
{
    private const int count = 10;

    //[Benchmark]
    //public void UseSeriLog()
    //{
    //    using var SeriLog = new Serilog.LoggerConfiguration()
    //        .WriteTo.File("./Logs/serilog.log", rollingInterval: RollingInterval.Minute)
    //        .CreateLogger();

    //    for(var i = 0; i < count; i++)
    //    {
    //        SeriLog.Information("Use SeriLog");
    //    }
    //}

    [Benchmark]
    public void UseLogable()
    {
        using var logger = new LoggerConfiguration()
            .Sink.File("./Logs/logable.log", options =>
            {
                options.Frequency = RollFrequency.Minute;
            })
            .CreateLogger();

        for(var i = 0; i < count; i++)
        {
            logger.Information("Use Logable");
        }
    }

    public void Validate()
    {
        //UseSeriLog();

        UseLogable();
    }
}
