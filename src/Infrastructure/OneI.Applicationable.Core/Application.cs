namespace OneI.Applicationable;

using System;
using OneI.Applicationable.Internal;

public static class Application
{
    public static IApplicationBuilder CreateBuilder() => new ApplicationBuilder(new());

    public static IApplicationBuilder CreateBuilder(string[]? args) => new ApplicationBuilder(new()
    {
        Arguments = args
    });

    public static IApplicationBuilder CreateBuilder(Action<ApplicationOptions> configure)
    {
        var options = new ApplicationOptions();

        configure(options);

        return new ApplicationBuilder(options);
    }
}
