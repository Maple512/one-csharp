namespace OneI.Applicationable;

using System.Threading.Tasks;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = Application.CreateBuilder();

        var app = builder.Build();

        await app.RunAsync();

        return 0;
    }
}
