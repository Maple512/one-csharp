namespace OneI.Applicationable;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IApplication
{
    IServiceProvider ServiceProvider { get; }

    ValueTask StartAsync(CancellationToken cancellationToken = default);

    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
