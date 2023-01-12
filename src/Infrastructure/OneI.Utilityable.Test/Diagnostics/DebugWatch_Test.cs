namespace OneI.Diagnostics;

public class DebugWatch_Test
{
    [Fact]
    public async void stop_watch()
    {
        var output = new StringWriter();

        DebugWatch.Mark();

        await Task.Delay(2 * 1000);

        DebugWatch.Stop();

        await Task.Delay(2 * 1000);

        DebugWatch.Mark();

        await Task.Delay(2 * 1000);

        DebugWatch.EndAndReport(s => output.Write(s));

        var result = output.ToString();
    }
}
