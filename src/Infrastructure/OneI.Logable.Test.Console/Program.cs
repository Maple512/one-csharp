namespace OneI.Logable;

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

        var message = $"这是一条消息";

        while(true)
        {
            // TODO: 有bug，两个string不会触发源代码生成器
            // TODO: 需要有一个纯异常的方法   logger.Warning(new Exception());

            try
            {
                throw new Exception("需要有一个纯异常的方法   logger.Warning(new Exception());");
            }
            catch(Exception ex)
            {
                logger.Verbose("{$Msg} {1} {2} {asdf} {0}", message, 3, new List<object>(), ex);
            }
        }
    }
}
