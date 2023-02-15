namespace OneI.Applicationable;

using System.Threading.Tasks;

public class Application_Test
{
    [Fact]
    public async Task create_appAsync()
    {
        var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var builder = Application.CreateBuilder();

        var app = builder.Build();

        await app.RunAsync(source.Token);
    }
}
