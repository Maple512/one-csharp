namespace OneI.Logable;

using System.IO;
using System.Text;
using System.Threading.Tasks;

public class Log_test
{
    [Fact]
    public async Task write_log_message()
    {
        // 过滤 + 格式化 + 输出

        var logger = new LoggerBuilder()
            .Filter(context =>
            {
                return Task.FromResult(context.Content.Level >= LogLevel.Information);
            })
            .Branch(async context =>
            {
                if(context.Content.Level == LogLevel.Debug)
                {
                    await WriteToAsync(context.Content.Text, Path.Combine(TestHelpler.LogFolder, $"{context.Content.Level}.txt"));
                }
            })
            .Branch(async context =>
            {
                if(context.Content.Level == LogLevel.Information)
                {
                    await WriteToAsync(context.Content.Text, Path.Combine(TestHelpler.LogFolder, $"{context.Content.Level}.txt"));
                }
            })
            .Build();

        await logger.WriteAsync(LogLevel.Verbose, "asdjf {0} werwer", 1);

        await logger.WriteAsync(new LogContent(LogLevel.Debug, "测试日志 Debug"));
        await logger.WriteAsync(new LogContent(LogLevel.Information, "测试日志 Information"));
    }

    private static async Task WriteToAsync(string? text, string path)
    {
        var directory = Path.GetDirectoryName(path);

        IOTools.CreateDirectory(directory!);

        await File.AppendAllTextAsync(path, text, Encoding.UTF8);
    }

    private class User
    {
        public string Id { get; set; }
    }
}
