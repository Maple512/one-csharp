namespace OneI.Applicationable;

using System.Threading.Tasks;
using OneI.Logable.Consoles;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = Application.CreateBuilder()
            .ConfigureLogger((context, logger) =>
            {
                _ = logger
                .Template.Default("{Timestamp:yyyy-MM-dd HH:mm:ss}|{Level}|{SourceContext}|{Message}{NewLine}")
                .Sink.UseConsole();
            });

        var app = builder.Build();

        await app.RunAsync();

        return 0;
    }
}
