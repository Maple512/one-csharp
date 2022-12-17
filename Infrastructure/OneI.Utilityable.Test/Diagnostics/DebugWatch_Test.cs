namespace OneI.Diagnostics;

public class DebugWatch_Test
{
    [Fact]
    public async void stop_watch()
    {
        DebugWatch.Start();

        DebugWatch.Mark();
        await Task.Delay(10);
        DebugWatch.Mark();
        await Task.Delay(10);
        DebugWatch.Mark();
        await Task.Delay(10);
        DebugWatch.Mark();

        DebugWatch.EndAndReport();
    }
}
