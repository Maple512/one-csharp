namespace OneI.Hostable;

public class Host_Test
{
    [Fact]
    public async Task stop_with_cancellation()
    {
        var builder = new HostBuilder();
        using var host = builder.Build();
        await host.StartAsync();
        var cts = new CancellationTokenSource();
        cts.Cancel();
        cts.IsCancellationRequested.ShouldBeTrue();
        await host.StopAsync(cts.Token);
    }
}
