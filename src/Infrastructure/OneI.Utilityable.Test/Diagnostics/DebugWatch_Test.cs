namespace OneI.Diagnostics;

public class DebugWatch_Test
{
    [Fact]
    public async void stop_watch()
    {
        var output = new StringWriter();

        DebugWatcher.Mark();

        await Task.Delay(2 * 10);

        DebugWatcher.Stop();

        await Task.Delay(2 * 10);

        DebugWatcher.Mark();

        await Task.Delay(2 * 10);

        var totalSeconds = 0D;
        DebugWatcher.EndAndReport((s, total, _) =>
        {
            output.Write(s);

            totalSeconds = total;
        });

        totalSeconds.ShouldBeGreaterThan(TimeSpan.FromMilliseconds(40).TotalSeconds);

        Debug.WriteLine(output.ToString());
    }
}
