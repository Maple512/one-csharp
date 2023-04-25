namespace OneI.Logable;

using System;
using System.IO;
using OneT.Common;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var file = Path.Combine(TestTools.GetCSProjectDirecoty(), $"Logs/RollFile_log.log");

        using var logger = new LoggerConfiguration()
                           .Sink.RollFile(file, configure =>
                           {
                               configure.Frequency = RollFrequency.Minute;
                               configure.ExpiredTime = TimeSpan.FromHours(1);
                               configure.CountLimit = 100;
                               configure.SizeLimit = 1024;
                           })
                           .CreateLogger();

        logger.Debug(new Exception(), $"asdfasdf{Guid.NewGuid():N}");

        while(true)
        {
            logger.Error(" {0} {1} {2}", 1, 3, default(int?));
        }
    }
}
