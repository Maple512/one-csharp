namespace OneI.Hostable;

using System.Globalization;
using Microsoft.Extensions.Configuration;

public class HostOptions
{
    /// <summary>
    /// 关闭<see cref="IHost"/>时的超时时间
    /// </summary>
    public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <inheritdoc cref="OneI.Hostable.BackgroundServiceExceptionBehavior"/>
    public BackgroundServiceExceptionBehavior BackgroundServiceExceptionBehavior { get; set; } = BackgroundServiceExceptionBehavior.StopHost;

    internal void Initialize(IConfiguration configuration)
    {
        var timeout = configuration[HostableConstants.HostOptions.ShutdownTimeout];

        if(timeout is { Length: > 0 }
        && int.TryParse(timeout, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds))
        {
            ShutdownTimeout = TimeSpan.FromSeconds(seconds);
        }
    }
}
